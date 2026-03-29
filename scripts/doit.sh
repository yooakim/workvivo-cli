#!/bin/bash
# dotnet run -- spaces users 142819 --csv --all > rs-stockholm.csv

tail -n +2 rs-stockholm-spaces.csv | while IFS=, read -r id name; do
    filename="${id}-${name// /}.csv"
    echo "Fetching '$name' (id=$id) -> $filename"
    dotnet run --project ../src/workvivo-cli -- spaces users "$id" --csv --all > "$filename"
done
