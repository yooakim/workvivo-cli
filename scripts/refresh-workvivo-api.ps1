<#
.SYNOPSIS
    Refresh the local Workvivo API spec from developer.workvivo.com.

.DESCRIPTION
    Pulls the public Postman v2.0.0 collection that powers the Workvivo
    developer portal, converts it to an OpenAPI 3.0 YAML spec, and
    regenerates docs/Workvivo-API.md as a concise human index.

    Produces (under docs/):
      - workvivo-api.postman.json   raw Postman collection (source of truth)
      - workvivo-api.openapi.yaml   converted OpenAPI 3.0 spec
      - Workvivo-API.md             human-readable index pointing at the YAML

.NOTES
    Requires Node.js on PATH (uses `npx postman-to-openapi`). No global
    install is performed.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$root          = Split-Path -Parent $PSScriptRoot
$docsDir       = Join-Path $root 'docs'
$postmanPath   = Join-Path $docsDir 'workvivo-api.postman.json'
$openapiPath   = Join-Path $docsDir 'workvivo-api.openapi.yaml'
$indexPath     = Join-Path $docsDir 'Workvivo-API.md'
$collectionUrl = 'https://developer.workvivo.com/api/collections/8787428/Uz59PKx9?environment=8787428-32273f4c-a065-4989-a260-60c5de910dc0&segregateAuth=true&versionTag=latest'

if (-not (Test-Path $docsDir)) { New-Item -ItemType Directory -Path $docsDir | Out-Null }

# Download raw bytes — PowerShell's ConvertFrom-Json | ConvertTo-Json
# round-trip mangles multi-line strings inside response.body fields and
# nearly doubles the file size, so persist the raw response verbatim.
Write-Host "[1/4] Downloading Postman collection..."
$response = Invoke-WebRequest -Uri $collectionUrl -UseBasicParsing
[System.IO.File]::WriteAllText($postmanPath, $response.Content, [System.Text.UTF8Encoding]::new($false))
Write-Host "      -> $postmanPath ($([math]::Round((Get-Item $postmanPath).Length / 1KB)) KB)"

Write-Host "[2/4] Converting Postman -> OpenAPI 3.0..."

# Sanitize via Node: postman-to-openapi throws on response examples whose
# body isn't valid JSON. Wrap those as JSON strings so conversion succeeds
# while keeping the example data.
$sanitizedPath = Join-Path $env:TEMP 'workvivo-sanitized.json'
$sanitizeScript = Join-Path $env:TEMP 'workvivo-sanitize.cjs'
@'
const fs = require('fs');
const [, , inp, out] = process.argv;
const pm = JSON.parse(fs.readFileSync(inp, 'utf8'));
let bodyFixed = 0, urlFixed = 0;

// Replace human placeholders like "{your Workvivo base URL}" with a valid
// hostname so postman-to-openapi's URL parsing succeeds. Only matches
// single-brace placeholders containing whitespace — leaves both Postman
// vars {{var}} and OpenAPI-style path params like {id}, {external_id} alone.
function repairUrlString(s) {
  return s.replace(/\{[^{}]*\s[^{}]*\}/g, 'your-tenant.example.com');
}

function walk(items) {
  for (const it of items || []) {
    if (it.request && it.request.url) {
      const u = it.request.url;
      if (typeof u === 'string') {
        const fixed = repairUrlString(u);
        if (fixed !== u) { it.request.url = fixed; urlFixed++; }
      } else if (u.raw) {
        const fixed = repairUrlString(u.raw);
        if (fixed !== u.raw) {
          u.raw = fixed;
          if (Array.isArray(u.host)) u.host = u.host.map(h => repairUrlString(h));
          urlFixed++;
        }
      }
    }
    if (it.response) {
      for (const r of it.response) {
        if (typeof r.body === 'string' && r.body.length > 0) {
          try { JSON.parse(r.body); }
          catch { r.body = JSON.stringify(r.body); bodyFixed++; }
        }
      }
    }
    if (it.item) walk(it.item);
  }
}
walk(pm.item);
fs.writeFileSync(out, JSON.stringify(pm));
console.log('      wrapped ' + bodyFixed + ' non-JSON response body example(s), repaired ' + urlFixed + ' URL(s)');
'@ | Set-Content -Path $sanitizeScript -Encoding utf8

$optionsPath = Join-Path $env:TEMP 'workvivo-p2o-options.json'
@{
    info    = @{ title = 'Workvivo API'; version = '1.0.0' }
    servers = @(@{ url = 'https://api.workvivo.com' })
} | ConvertTo-Json -Depth 10 -Compress | Set-Content -Path $optionsPath -Encoding utf8

try {
    & node $sanitizeScript $postmanPath $sanitizedPath
    if ($LASTEXITCODE -ne 0) { throw "sanitize step exited with $LASTEXITCODE" }

    # p2o echoes the entire spec to stdout in addition to writing -f; drop it.
    & npx -y postman-to-openapi@latest $sanitizedPath -f $openapiPath -o $optionsPath > $null 2>&1
    if ($LASTEXITCODE -ne 0) { throw "postman-to-openapi exited with $LASTEXITCODE" }
} finally {
    Remove-Item $sanitizedPath, $sanitizeScript, $optionsPath -Force -ErrorAction SilentlyContinue
}
Write-Host "      -> $openapiPath"

Write-Host "[3/4] Regenerating Markdown index..."

# Read the raw Postman JSON via ConvertFrom-Json (read-only — no round-trip
# write, so the body-mangling issue doesn't apply here).
$collection = Get-Content -Path $postmanPath -Raw | ConvertFrom-Json

function Get-Operations {
    param([object[]]$Items, [string[]]$Crumbs = @())

    $ops = New-Object System.Collections.Generic.List[object]
    $topLevel = @('API','User Contextual API') # collapse: strip these as group prefix
    foreach ($item in $Items) {
        if ($null -ne $item.request) {
            $req = $item.request
            $method = if ($req.method) { $req.method.ToUpper() } else { 'GET' }
            $raw = $null
            if ($req.url -is [string]) { $raw = $req.url }
            elseif ($req.url.raw)      { $raw = $req.url.raw }
            elseif ($req.url.path)     { $raw = '/' + ($req.url.path -join '/') }
            $path = if ($raw) { ($raw -replace '\?.*$','') -replace '^https?://[^/]+','' } else { '(no url)' }

            $tagCrumbs = @($Crumbs)
            if ($tagCrumbs.Count -gt 0 -and $topLevel -contains $tagCrumbs[0]) {
                $tagCrumbs = @($tagCrumbs | Select-Object -Skip 1)
            }
            $tag = if ($tagCrumbs.Count -gt 0) { $tagCrumbs -join ' > ' } else { $Crumbs[0] }

            $ops.Add([pscustomobject]@{
                Tag    = $tag
                Method = $method
                Path   = $path
                Name   = $item.name
            })
        }
        if ($item.item) {
            foreach ($child in (Get-Operations -Items $item.item -Crumbs ($Crumbs + $item.name))) {
                $ops.Add($child)
            }
        }
    }
    return ,$ops
}

$ops = Get-Operations -Items $collection.item

$priorityTags = @('Chats','Users','Spaces','Teams','Documents','Events')
$grouped = $ops | Group-Object Tag
$tagOrder = @()
# Priority section + any nested subsections (e.g. "Events > Comments").
foreach ($p in $priorityTags) {
    foreach ($g in ($grouped | Sort-Object Name)) {
        if ($g.Name -eq $p -or $g.Name.StartsWith("$p > ")) {
            if ($tagOrder -notcontains $g.Name) { $tagOrder += $g.Name }
        }
    }
}
foreach ($g in ($grouped | Sort-Object Name)) {
    if ($tagOrder -notcontains $g.Name) { $tagOrder += $g.Name }
}

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine('# Workvivo API — Index')
[void]$sb.AppendLine()
[void]$sb.AppendLine('Generated index of the Workvivo REST API. **Do not hand-edit** — regenerate with')
[void]$sb.AppendLine('`scripts/refresh-workvivo-api.ps1`.')
[void]$sb.AppendLine()
[void]$sb.AppendLine('## Files')
[void]$sb.AppendLine()
[void]$sb.AppendLine('| File | Purpose |')
[void]$sb.AppendLine('|------|---------|')
[void]$sb.AppendLine('| `workvivo-api.postman.json` | Raw Postman v2.0.0 collection pulled from developer.workvivo.com (source of truth) |')
[void]$sb.AppendLine('| `workvivo-api.openapi.yaml` | Converted OpenAPI 3.0 spec — full schemas, examples, params |')
[void]$sb.AppendLine('| `Workvivo-API.md`           | This index (operations grouped by tag) |')
[void]$sb.AppendLine()
[void]$sb.AppendLine('## Base URL & auth')
[void]$sb.AppendLine()
[void]$sb.AppendLine('- Server: `https://api.workvivo.com` (the `/v1` version prefix is part of each path below).')
[void]$sb.AppendLine('- Required headers on every request:')
[void]$sb.AppendLine('  - `Authorization: Bearer <token>`')
[void]$sb.AppendLine('  - `Workvivo-Id: <workvivo-id>`')
[void]$sb.AppendLine('  - `Accept: application/json`')
[void]$sb.AppendLine()
[void]$sb.AppendLine("Total operations: $($ops.Count) across $($tagOrder.Count) sections.")
[void]$sb.AppendLine()
[void]$sb.AppendLine('## Operations')
[void]$sb.AppendLine()

foreach ($tag in $tagOrder) {
    $tagOps = $grouped | Where-Object Name -EQ $tag | Select-Object -ExpandProperty Group
    if (-not $tagOps) { continue }
    [void]$sb.AppendLine("### $tag")
    [void]$sb.AppendLine()
    foreach ($op in $tagOps) {
        $line = "- ``$($op.Method.PadRight(6)) $($op.Path)``"
        if ($op.Name -and $op.Name -ne $op.Path) { $line += " — $($op.Name)" }
        [void]$sb.AppendLine($line)
    }
    [void]$sb.AppendLine()
}

Set-Content -Path $indexPath -Value $sb.ToString() -Encoding utf8
Write-Host "      -> $indexPath"

Write-Host "[4/4] Done. $($ops.Count) operations indexed across $($tagOrder.Count) sections."
