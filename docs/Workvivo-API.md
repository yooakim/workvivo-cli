# Workvivo API — Index

Generated index of the Workvivo REST API. **Do not hand-edit** — regenerate with
`scripts/refresh-workvivo-api.ps1`.

## Files

| File | Purpose |
|------|---------|
| `workvivo-api.postman.json` | Raw Postman v2.0.0 collection pulled from developer.workvivo.com (source of truth) |
| `workvivo-api.openapi.yaml` | Converted OpenAPI 3.0 spec — full schemas, examples, params |
| `Workvivo-API.md`           | This index (operations grouped by tag) |

## Base URL & auth

- Server: `https://api.workvivo.com` (the `/v1` version prefix is part of each path below).
- Required headers on every request:
  - `Authorization: Bearer <token>`
  - `Workvivo-Id: <workvivo-id>`
  - `Accept: application/json`

Total operations: 346 across 68 sections.

## Operations

### Chats > Bots

- `POST   /v1/chat/bots` — Create a chat bot
- `PUT    /v1/chat/bots/:bot_userid` — Update a chat bot
- `DELETE /v1/chat/bots/:bot_userid` — Delete a chat bot
- `GET    /v1/chat/bots` — Get a collection of chat bots
- `GET    /v1/chat/bots/:id` — Get a single chat bot by ID

### Chats > Bots > Channel

- `POST   /v1/chat/bots/channels` — Create a bot Channel

### Chats > Bots > Message

- `POST   /v1/chat/bots/message` — Create a bot message
- `POST   /v1/chat/bots/message` — Create a bot file attachment
- `POST   /v1/chat/bots/message` — Create a bot image attachment
- `POST   /v1/chat/bots/message` — Create a bot video attachment
- `POST   /v1/chat/bots/message` — Create a bot quick reply
- `POST   /v1/chat/bots/message` — Create a bot card

### Chats > Channels

- `GET    /v1/chat/channels/:channel_url` — Get a single chat channel by channel url
- `POST   /v1/chat/channels` — Create a chat channel by Workvivo ID
- `POST   /v1/chat/by-external-id/channels` — Create a chat channel by external ID
- `GET    /v1/chat/channels` — Get a collection of chat channels

### Chats > Channels > Members

- `PATCH  /v1/chat/channels/:channel_url/members` — Add users to a chat channel by ID
- `PATCH  /v1/chat/channels/:channel_url/members` — Add users  to a chat channel by external ID
- `PATCH  /v1/chat/channels/:channel_url/members` — Add chatbots to a chat channel by ID

### Users

- `GET    /v1/users` — Get a collection of users
- `GET    /v1/users/:id` — Get a single user by Workvivo ID
- `GET    /v1/users/by-email/:email` — Get a single user by email
- `GET    /v1/users/by-external-id/:external_id` — Get a single user by external ID
- `PUT    /v1/users/:id/profile-photo` — Update a profile photo for a user
- `PUT    /v1/users/by-external-id/:external-id/profile-photo` — Update a profile photo for a user by external ID
- `PUT    /v1/users/frontline-status` — Update frontline status and access
- `POST   /v1/users/:user_id/badges/:badge_id` — Assign a badge to a user
- `POST   /v1/users/by-external-id/:external_id/badges/:badge_id` — Assign a badge to a user by external ID
- `DELETE /v1/users/:user_id/badges/:badge_id` — Unassign a badge from a user
- `DELETE /v1/users/by-external-id/:external_id/badges/:badge_id` — Unassign a badge from a user by external ID

### Spaces

- `GET    /v1/spaces` — Get a collection of spaces
- `GET    /v1/spaces/:id` — Get a single space by ID
- `GET    /v1/spaces/categories` — Get a collection of space categories
- `GET    /v1/spaces/:id/users` — Get a collection of space users
- `POST   /v1/spaces` — Create a space
- `PUT    /v1/spaces/:id` — Update a space
- `PATCH  /v1/spaces/:id/users` — Add or remove users to/from a space by ID
- `PATCH  /v1/spaces/by-external-id/:id/users` — Add or remove users to/from a space by external ID
- `POST   /v1/spaces/:id/archive` — Archive a space
- `POST   /v1/spaces/:id/unarchive` — Unarchive a space
- `GET    /v1/jwt/spaces` — Get a collection of spaces

### Spaces > Default Spaces

- `GET    /v1/spaces/{{space_id}}/default` — Get a collection of default spaces
- `PUT    /v1/spaces/{{space_id}}/default` — Update default spaces
- `DELETE /v1/spaces/{{space_id}}/default` — Delete default spaces

### Teams

- `GET    /v1/teams` — Get a collection of teams
- `GET    /v1/users/:id/teams` — Get a collection of teams a given user is in (by id)
- `GET    /v1/users/by-email/:email/teams` — Get a collection of teams a given user is in (by email)
- `GET    /v1/users/by-external-id/:external_id/teams` — Get a collection of teams a given user is in (by external ID)
- `GET    /v1/teams/:id` — Get a single team by Workvivo ID
- `GET    /v1/teams/by-external-id/:external_id` — Get a single team by external id
- `PATCH  /v1/teams/:id/users` — Add or remove users to / from a team by id
- `PATCH  /v1/teams/by-external-id/:external_id/users` — Add or remove users to / from a team by external id
- `POST   /v1/teams` — Create a team
- `GET    /v1/teams/types` — Get a collection of team types

### Documents

- `GET    /v1/documents/:id` — Get a single document by ID
- `GET    /v1/documents` — Get a collection of documents

### Events

- `GET    /v1/events` — Get a collection of events
- `GET    /v1/events/:id` — Get a single event by ID
- `GET    /v1/events/by-external-id/:external_id` — Get a single event by external ID
- `POST   /v1/events` — Create an event
- `PUT    /v1/events/:id` — Update an event
- `PUT    /v1/events/by-external-id/:external_id` — Update an event by extenal ID
- `DELETE /v1/events/:id` — Delete an event
- `DELETE /v1/events/by-external-id/:external_id` — Delete an event by external id
- `GET    /v1/jwt/events` — Get a collection of events

### Events > Attendees

- `POST   /v1/events/:id/attendees` — Add attendee to event by ID
- `POST   /v1/events/by-external-id/:external_id/attendees` — Add attendee to event by external ID
- `DELETE /v1/events/:id/attendees/:user_id` — Delete attendee from event by ID
- `DELETE /v1/events/by-external-id/:external_id/attendees/:user_external_id` — Delete attendee from event by external ID

### Events > Comments

- `POST   /v1/events/:event-id/comments` — Create a comment
- `PUT    /v1/events/:event-id/comments/:comment-id` — Update a comment by ID
- `PUT    /v1/events/by-external-id/:event_external_id/comments/:comment_external_id` — Update a comment by external ID
- `GET    /v1/events/:event-id/comments` — Get all comments
- `GET    /v1/events/by-external-id/:external_id/comments` — Get all comments by external ID
- `GET    /v1/events/:event-id/comments/:comment-id/replies` — Get all comment replies
- `GET    /v1/events/by-external-id/:external_id/comments/:external-comments-id/replies` — Get all comment replies by external ID
- `GET    /v1/events/:event-id/comments/:comment-id` — Get a single Comment by ID
- `GET    /v1/events/by-external-id/:event_external_id/comments/:comment_external_id` — Get a single Comment by external ID
- `DELETE /v1/events/:event-id/comments/:comment-id` — Delete a comment by ID
- `DELETE /v1/events/by-external-id/:event-external-id/comments/:comment-external-id` — Delete a comment by external ID
- `POST   /v1/events/:event_id/comments/:comment_id/report` — Report a comment by ID
- `POST   /v1/events/by-external-id/:event_external_id/comments/:comment_external_id/report` — Report a comment by external ID

### Events > Comments > Likes

- `GET    /v1/events/:event-id/comments/:comment-id/likes` — Get likes for a comment by ID
- `GET    /v1/events/by-external-id/:event_external_id/comments/:external_comment_id/likes` — Get likes for a comment by external ID
- `GET    /v1/events/:event-id/comments/:comment-id/likes/count` — Get likes count for a comment by ID
- `GET    /v1/events/by-external-id/:event_external_id/comments/:external_comment_id/likes/count` — Get likes count for a comment by external ID
- `POST   /v1/events/:id/comments/:comment_id/likes` — Create like for a comment by ID
- `POST   /v1/events/by-external-id/:event_external_id/comments/:comment_external_id/likes` — Create like for a comment by external ID
- `DELETE /v1/events/:event_id/comments/:comment_id/likes` — Delete like for a comment by ID
- `DELETE /v1/events/by-external-id/:event_external_id/comments/:comment_external_id/likes` — Delete like for a comment by external ID Copy

### Events > Likes

- `GET    /v1/events/:id/likes` — Get likes for an event by ID
- `GET    /v1/events/by-external-id/:external_id/likes` — Get likes for an event by external ID
- `GET    /v1/events/:id/likes/count` — Get likes count for an event by ID
- `GET    /v1/events/by-external-id/:external_id/likes/count` — Get likes count for an event by external ID
- `POST   /v1/events/:id/likes` — Create like for an event by ID
- `POST   /v1/events/by-external-id/:external_id/likes` — Create like for an event by external ID
- `DELETE /v1/events/:id/likes` — Delete like for an event by ID
- `DELETE /v1/events/by-external-id/:external_id/likes` — Delete like for an event by external ID

### Events > Reactions

- `GET    /v1/events/:id/reactions/count` — Get reactions count for an event by ID
- `GET    /v1/events/by-external-id/:external_id/reactions/count` — Get reactions count for an event by external ID
- `POST   /v1/events/:id/reactions` — Add a reaction to an event by ID
- `POST   /v1/events/by-external-id/:external_id/reactions` — Add a reaction to an event by external ID
- `DELETE /v1/events/:id/reactions` — Remove a reaction from an event by ID
- `DELETE /v1/events/by-external-id/:external_id/reactions` — Remove a reaction from an event by external ID

### Activities

- `GET    /v1/activities` — Get a collection of activities

### Advanced Search

- `GET    /v1/jwt/advanced-search` — Get a collection of searched items

### Apps

- `GET    /v1/apps/:id` — Get a single App by ID
- `GET    /v1/apps` — Get a collection of Apps

### Articles

- `GET    /v1/articles` — Get a collection of articles
- `GET    /v1/articles/:id` — Get a single article by ID
- `GET    /v1/articles/by-external-id/:external_id` — Get a single article by external ID
- `POST   /v1/articles` — Create an article
- `PUT    /v1/articles/:id` — Update an article by ID
- `PUT    /v1/articles/by-external-id/:external_id` — Update an article by external ID
- `DELETE /v1/articles/:id` — Delete an article
- `DELETE /v1/articles/by-external-id/:external_id` — Delete an article by external ID
- `GET    /v1/jwt/articles` — Get a collection of articles

### Articles > Acknowledgements

- `GET    /v1/articles/:id/acknowledgement` — Get an acknowledgement by ID
- `GET    /v1/articles/by-external-id/:external_id/acknowledgement` — Get an acknowledgement by external ID

### Articles > Acknowledgements > Users

- `GET    /v1/articles/:id/acknowledgement/users` — Get a collection of acknowledgement users by ID
- `GET    /v1/articles/by-external-id/:id/acknowledgement/users` — Get a collection of acknowledgement users by external ID

### Articles > Comments

- `POST   /v1/articles/:article-id/comments` — Create a comment
- `PUT    /v1/articles/:article-id/comments/:comment-id` — Update a comment by ID
- `PUT    /v1/articles/by-external-id/:article_external_id/comments/:comment_external_id` — Update a comment by external ID
- `GET    /v1/articles/:article-id/comments` — Get all comments
- `GET    /v1/articles/:article-id/comments/:comment-id/replies` — Get all comment replies
- `GET    /v1/articles/by-external-id/:external_id/comments` — Get all comments by external ID
- `GET    /v1/articles/by-external-id/:external_id/comments/:external_comment_id/replies` — Get all comment replies by external ID
- `GET    /v1/articles/:article-id/comments/:comment-id` — Get a single Comment by ID
- `GET    /v1/articles/by-external-id/:article_external_id/comments/:comment_external_id` — Get a single Comment by external ID
- `DELETE /v1/articles/:article-id/comments/:comment-id` — Delete a comment by ID
- `DELETE /v1/articles/by-external-id/:article_external_id/comments/:comment_external_id` — Delete a comment by external ID
- `POST   /v1/articles/:article_id/comments/:comment_id/report` — Report a comment by ID
- `POST   /v1/articles/by-external-id/:article_external_id/comments/:comment_external_id/report` — Report a comment by external ID

### Articles > Comments > Likes

- `GET    /v1/articles/:article-id/comments/:comment-id/likes` — Get likes for a comment by ID
- `GET    /v1/articles/by-external-id/:article_external_id/comments/:external_comment_id/likes` — Get likes for a comment by external ID
- `GET    /v1/articles/:article-id/comments/:comment-id/likes/count` — Get likes count for a comment by ID
- `GET    /v1/articles/by-external-id/:article_external_id/comments/:external_comment_id/likes/count` — Get likes count for a comment by external ID
- `POST   /v1/articles/:id/comments/:comment_id/likes` — Create like for a comment by ID
- `POST   /v1/article/by-external-id/:article_external_id/comments/:comment_external_id/likes` — Create like for a comment by external ID
- `DELETE /v1/articles/:article_id/comments/:comment_id/likes` — Delete like for a comment by ID
- `DELETE /v1/articles/by-external-id/:article_external_id/comments/:comment_external_id/likes` — Delete like for a comment by external ID Copy

### Articles > Language Variants

- `POST   /v1/articles/:article_id/language-variant` — Create Language Variant
- `POST   /v1/articles/by-external-id/:external_id/language-variant` — Create Language Variant by article external ID
- `PUT    /v1/articles/:article_id/language-variant/:language_code` — Update Language Variant by Article ID
- `PUT    /v1/articles/by-external-id/:external_id/language-variant/:language_code` — Update Language Variant by Article external ID
- `DELETE /v1/articles/:id/language-variant/:language_code` — Delete a language variant
- `DELETE /v1/articles/by-external-id/:external_id/language-variant/:language_code` — Delete a language variant by article external ID
- `POST   /v1/articles/settings/default-api-translation-language` — Update Api Default Translation Language

### Articles > Likes

- `GET    /v1/articles/:id/likes` — Get likes for an article by ID
- `GET    /v1/articles/:id/likes/count` — Get likes count for an article by ID
- `GET    /v1/articles/by-external-id/:external_id/likes` — Get likes for an article by external ID
- `GET    /v1/articles/by-external-id/:external_id/likes/count` — Get likes count for an article by external ID
- `POST   /v1/articles/:id/likes` — Create like for an article by ID
- `POST   /v1/articles/by-external-id/:external_id/likes` — Create like for an article by external ID
- `DELETE /v1/articles/:id/likes` — Delete like for an article by ID
- `DELETE /v1/articles/by-external-id/:external_id/likes` — Delete like for an article by external ID

### Articles > Reactions

- `GET    /v1/articles/:id/reactions/count` — Get reactions count for an article by ID
- `GET    /v1/articles/by-external-id/:external_id/reactions/count` — Get reactions count for an article by external ID
- `POST   /v1/articles/:id/reactions` — Add a reaction to an article by ID
- `POST   /v1/articles/by-external-id/:external_id/reactions` — Add a reaction to an article by external ID
- `DELETE /v1/articles/:id/reactions` — Remove a reaction from an article by ID
- `DELETE /v1/articles/by-external-id/:external_id/reactions` — Remove a reaction from an article by external ID

### Badges

- `GET    /v1/badges` — Get a collection of badges
- `GET    /v1/badges/:id` — Get a single badge by ID
- `POST   /v1/badges` — Create a badge
- `PUT    /v1/badges/:id` — Update a badge
- `DELETE /v1/badges/:id` — Delete a badge

### Billboards

- `GET    /v1/billboards` — Get a collection of billboards
- `GET    /v1/billboards/:id` — Get a single billboard by ID

### Discount Codes

- `GET    /v1/discount-codes` — Get a collection of discount codes
- `GET    /v1/discount-codes/:id` — Get a single discount code
- `POST   /v1/discount-codes` — Create a discount code
- `DELETE /v1/discount-codes/:id` — Delete a discount code
- `PUT    /v1/discount-codes/:id` — Update a discount code

### Discount Codes > Users

- `GET    /v1/discount-codes/:id/users` — Get a collection of discount code users
- `GET    /v1/discount-codes/:id/users/:user_id` — Get a single discount code user
- `GET    /v1/discount-codes/:id/users/by-external-id/:user_external_id` — Get a single discount code user by external user ID
- `POST   /v1/discount-codes/:id/users` — Create discount code users
- `PUT    /v1/discount-codes/:id/users` — Update discount code users
- `DELETE /v1/discount-codes/:id/users/:user_id` — Delete a discount code user

### Goals

- `GET    /v1/goals` — Get a collection of goals
- `GET    /v1/goals/:id` — Get a single goal by ID

### JWT SSO

- `GET    /proxy/redirect/sso/` — Single Sign On with JWT
- `GET    /proxy/redirect/sso/` — 401 - Unauthorized

### Kudos

- `GET    /v1/kudos` — Get a collection of kudos
- `GET    /v1/kudos/:id` — Get Kudo by ID
- `GET    /v1/kudos/by-external-id/:external_id` — Get Kudo by external ID
- `POST   /v1/kudos` — Create a kudo
- `PUT    /v1/kudos/:id` — Update a kudo by ID
- `PUT    /v1/kudos/by-external-id/:external_id` — Update a kudo by external ID
- `DELETE /v1/kudos/:id` — Delete a kudo by ID
- `DELETE /v1/kudos/by-external-id/:external_id` — Delete a kudo by External ID
- `POST   /v1/kudos/:id/report` — Report a kudo by ID
- `POST   /v1/kudos/by-external-id/:external_id/report` — Report a kudo by external ID

### Kudos > Acknowledgements

- `GET    /v1/kudos/:id/acknowledgement` — Get an acknowledgement by ID
- `GET    /v1/kudos/by-external-id/:external_id/acknowledgement` — Get an acknowledgement by external ID

### Kudos > Acknowledgements > Users

- `GET    /v1/kudos/:id/acknowledgement/users` — Get a collection of acknowledgement users by ID
- `GET    /v1/kudos/by-external-id/:external-id/acknowledgement/users` — Get a collection of acknowledgement users by external ID

### Kudos > Campaigns

- `GET    /v1/kudos/:id/campaigns` — Get campaigns by ID
- `GET    /v1/kudos/by-external-id/:external_id/campaigns` — Get campaigns by external ID

### Kudos > Comments

- `POST   /v1/kudos/:kudo-id/comments` — Create a comment
- `PUT    /v1/kudos/:kudo-id/comments/:comment-id` — Update a comment by ID
- `PUT    /v1/kudos/by-external-id/:kudo_external_id/comments/:comment_external_id` — Update a comment by external ID
- `GET    /v1/kudos/:kudo-id/comments` — Get all comments
- `GET    /v1/kudos/:kudo-id/comments/:comment-id/replies` — Get all comment replies
- `GET    /v1/kudos/by-external-id/:external_id/comments` — Get all comments by external ID
- `GET    /v1/kudos/by-external-id/:external_id/comments/:comment_external_id/replies` — Get all comment replies by external ID
- `GET    /v1/kudos/:kudo-id/comments/:comment-id` — Get a single Comment by ID
- `GET    /v1/kudos/by-external-id/:kudo_external_id/comments/:comment_external_id` — Get a single Comment by external ID
- `DELETE /v1/kudos/:kudo-id/comments/:comment-id` — Delete a comment by ID
- `DELETE /v1/kudos/by-external-id/:kudo_external_id/comments/:comment_external_id` — Delete a comment by external ID
- `POST   /v1/kudos/:kudo_id/comments/:comment_id/report` — Report a comment by ID
- `POST   /v1/kudos/by-external-id/:kudos_external_id/comments/:comment_external_id/report` — Report a comment by external ID

### Kudos > Comments > Likes

- `GET    /v1/kudos/:kudo-id/comments/:comment-id/likes` — Get likes for a comment by ID
- `GET    /v1/kudos/by-external-id/:kudo_external_id/comments/:external_comment_id/likes` — Get likes for a comment by external ID
- `GET    /v1/kudos/:kudo-id/comments/:comment-id/likes/count` — Get likes count for a comment by ID
- `GET    /v1/kudos/by-external-id/:kudo_external_id/comments/:external_comment_id/likes/count` — Get likes count for a comment by external ID
- `POST   /v1/kudos/:id/comments/:comment_id/likes` — Create like for a comment by ID
- `POST   /v1/kudos/by-external-id/:kudo_external_id/comments/:comment_external_id/likes` — Create like for a comment by external ID
- `DELETE /v1/kudos/:kudo_id/comments/:comment_id/likes` — Delete like for a comment by ID
- `DELETE /v1/kudos/by-external-id/:kudo_external_id/comments/:comment_external_id/likes` — Delete like for a comment by external ID Copy

### Kudos > Likes

- `GET    /v1/kudos/:id/likes` — Get likes for a kudo by ID
- `GET    /v1/kudos/by-external-id/:external_id/likes` — Get likes for a kudo by external ID
- `GET    /v1/kudos/:id/likes/count` — Get likes count for a kudo by ID
- `GET    /v1/kudos/by-external-id/:external_id/likes/count` — Get likes count for a kudo by external ID
- `POST   /v1/kudos/:id/likes` — Create like for a kudo by ID
- `POST   /v1/kudos/by-external-id/:external_id/likes` — Create like for an kudo by external ID
- `DELETE /v1/kudos/:id/likes` — Delete like for an kudo by ID
- `DELETE /v1/kudos/by-external-id/:external_id/likes` — Delete like for a kudo by external ID

### Kudos > Reactions

- `GET    /v1/kudos/:id/reactions/count` — Get reactions count for a kudo by ID
- `GET    /v1/kudos/by-external-id/:external_id/reactions/count` — Get reactions count for a kudo by external ID
- `POST   /v1/kudos/:id/reactions` — Add a reaction to a kudos by ID
- `POST   /v1/kudos/by-external-id/:external_id/reactions` — Add a reaction to a kudos by external ID
- `DELETE /v1/kudos/:id/reactions` — Remove a reaction from a kudos by ID
- `DELETE /v1/kudos/by-external-id/:external_id/reactions` — Remove a reaction from a kudos by external ID

### Livestreams

- `GET    /v1/livestreams` — Get a collection of livestreams
- `GET    /v1/livestreams/:id` — Get a livestream by ID

### Mega Menu

- `GET    /v1/mega-menu` — Get Mega Menu

### Notifications

- `GET    /v1/notifications` — Get a collection of notifications
- `POST   /v1/notifications` — Create a notification to be sent to users
- `GET    /v1/notifications/:id` — Get a single notification
- `PUT    /v1/notifications/:id` — Update a notification
- `DELETE /v1/notifications/:id` — Delete a notification

### Pages

- `GET    /v1/pages` — Get a collection of pages
- `POST   /v1/pages` — Create a page
- `PUT    /v1/pages/:id` — Update a page
- `PUT    /v1/pages/by-external-id/:external-id` — Update a page by external ID
- `DELETE /v1/pages/:id` — Delete a page
- `DELETE /v1/pages/by-external-id/:external-id` — Delete a page by external ID

### Pages > Language Variants

- `POST   /v1/pages/:page_id/language-variant` — Create a language variant
- `POST   /v1/pages/by-external-id/:external_id/language-variant` — Create a language variant by page external ID
- `PUT    /v1/pages/:page_id/language-variant/:language_code` — Update a language variant
- `PUT    /v1/pages/by-external-id/:external_id/language-variant/:language_code` — Update a language variant by page external ID
- `DELETE /v1/pages/:page_id/language-variant/:language_code` — Delete a language variant
- `DELETE /v1/pages/by-external-id/:external_id/language-variant/:language_code` — Delete a language variant by page external ID

### Ping

- `GET    /v1/jwt/ping` — Ping test

### Reactions

- `GET    /v1/reactions` — Get available reactions

### Roles

- `GET    /v1/roles` — Get a collection of roles
- `GET    /v1/roles/space` — Get a collection of space roles

### SIEM

- `GET    /v1/siem/:fromEpoch/:toEpoch` — Get SIEM event information
- `GET    /v1/siem/users/:userID/events/:fromEpoch/:toEpoch` — Get a Users SIEM event information

### Unwired > Users

- `POST   /v1/unwired/users/otp` — One Time Passcode

### Updates

- `GET    /v1/updates` — Get a collection of updates
- `GET    /v1/updates/:id` — Get Update by ID
- `GET    /v1/updates/by-external-id/:external_id` — Get Update by external ID
- `POST   /v1/updates` — Create an update
- `PUT    /v1/updates/:id` — Update an update by ID
- `PUT    /v1/updates/by-external-id/:external_id` — Update an update by external ID
- `DELETE /v1/updates/:id` — Delete an update by ID
- `DELETE /v1/updates/by-external-id/:external_id` — Delete an update by External ID
- `POST   /v1/updates/:id/report` — Report an update by ID
- `POST   /v1/updates/by-external-id/:external_id/report` — Report an update by external ID

### Updates > Acknowledgements

- `GET    /v1/updates/:id/acknowledgement` — Get an acknowledgement by ID
- `GET    /v1/updates/by-external-id/:external_id/acknowledgement` — Get an acknowledgement by external ID

### Updates > Acknowledgements > Users

- `GET    /v1/updates/:id/acknowledgement/users` — Get a collection of acknowledgement users by ID
- `GET    /v1/updates/by-external-id/:external_id/acknowledgement/users` — Get a collection of acknowledgement users by external ID

### Updates > Campaigns

- `GET    /v1/updates/:id/campaigns` — Get campaigns by ID
- `GET    /v1/updates/by-external-id/:external_id/campaigns` — Get campaigns by external ID

### Updates > Comments

- `POST   /v1/updates/:update-id/comments` — Create a comment
- `PUT    /v1/updates/:update-id/comments/:comment-id` — Update a comment by ID
- `PUT    /v1/updates/by-external-id/:update_external_id/comments/:comment_external_id` — Update a comment by external ID
- `GET    /v1/updates/:update-id/comments` — Get all comments
- `GET    /v1/updates/:update-id/comments/:comment-id/replies` — Get all comment replies
- `GET    /v1/updates/by-external-id/:external_id/comments` — Get all comments by external ID
- `GET    /v1/updates/by-external-id/:external_id/comments/:external-comments-id/replies` — Get all comment replies by external ID
- `GET    /v1/updates/:update-id/comments/:comment-id` — Get a single Comment by ID
- `GET    /v1/updates/by-external-id/:update_external_id/comments/:comment_external_id` — Get a single Comment by external ID
- `DELETE /v1/updates/:update-id/comments/:comment-id` — Delete a comment by ID
- `DELETE /v1/updates/by-external-id/:update_external_id/comments/:comment_external_id` — Delete a comment by external ID
- `POST   /v1/updates/:update_id/comments/:comment_id/report` — Report a comment by ID
- `POST   /v1/updates/by-external-id/:update_external_id/comments/:comment_external_id/report` — Report a comment by external ID

### Updates > Comments > Likes

- `GET    /v1/updates/:update-id/comments/:comment-id/likes` — Get likes for a comment by ID
- `GET    /v1/updates/by-external-id/:update_external_id/comments/:external_comment_id/likes` — Get likes for a comment by external ID
- `GET    /v1/updates/:update-id/comments/:comment-id/likes/count` — Get likes count for a comment by ID
- `GET    /v1/updates/by-external-id/:update_external_id/comments/:external_comment_id/likes/count` — Get likes count for a comment by external ID
- `POST   /v1/updates/:id/comments/:comment_id/likes` — Create like for a comment by ID
- `POST   /v1/updates/by-external-id/:update_external_id/comments/:comment_external_id/likes` — Create like for a comment by external ID
- `DELETE /v1/updates/:update_id/comments/:comment_id/likes` — Delete like for a comment by ID
- `DELETE /v1/updates/by-external-id/:update_external_id/comments/:comment_external_id/likes` — Delete like for a comment by external ID Copy

### Updates > Likes

- `GET    /v1/updates/:id/likes` — Get likes for a update by ID
- `GET    /v1/updates/by-external-id/:external_id/likes` — Get likes for a update by external ID
- `GET    /v1/updates/:id/likes/count` — Get likes count for a update by ID
- `GET    /v1/updates/by-external-id/:external_id/likes/count` — Get likes count for a update by external ID
- `POST   /v1/updates/:id/likes` — Create like for an update by ID
- `POST   /v1/updates/by-external-id/:external_id/likes` — Create like for an update by external ID
- `DELETE /v1/updates/:id/likes` — Delete like for an update by ID
- `DELETE /v1/updates/by-external-id/:external_id/likes` — Delete like for an update by external ID

### Updates > Reactions

- `GET    /v1/updates/:id/reactions/count` — Get reactions count for an update by ID
- `GET    /v1/updates/by-external-id/:external_id/reactions/count` — Get reactions count for an update by external ID
- `POST   /v1/updates/:id/reactions` — Add a reaction to an update by ID
- `POST   /v1/updates/by-external-id/:external_id/reactions` — Add a reaction to an update by external ID
- `DELETE /v1/updates/:id/reactions` — Remove a reaction from an update by ID
- `DELETE /v1/updates/by-external-id/:external_id/reactions` — Remove a reaction from an update by external ID

### Webhooks

- `GET    /v1/webhooks` — Get a collection of webhooks
- `GET    /v1/webhooks/:id` — Get a single webhook by ID

### Webhooks > Articles

- `POST   /webhooks/workvivo` — article.created
- `POST   /webhooks/workvivo` — article.updated
- `POST   /webhooks/workvivo` — article.published
- `POST   /webhooks/workvivo` — article.deleted
- `POST   /webhooks/workvivo` — article.archived

### Webhooks > Bookmarks

- `POST   /webhooks/workvivo` — bookmark.created
- `POST   /webhooks/workvivo` — bookmark.deleted

### Webhooks > Chat

- `POST   /webhooks/workvivo` — chat.message.created
- `POST   /webhooks/workvivo` — chat.message.deleted
- `POST   /webhooks/workvivo` — chat.channel.read
- `POST   /webhooks/workvivo` — chat.bot.channel.read

### Webhooks > Comments

- `POST   /webhooks/workvivo` — comment.created
- `POST   /webhooks/workvivo` — comment.updated
- `POST   /webhooks/workvivo` — comment.deleted

### Webhooks > Events

- `POST   /webhooks/workvivo` — event.created
- `POST   /webhooks/workvivo` — event.updated
- `POST   /webhooks/workvivo` — event.published
- `POST   /webhooks/workvivo` — event.deleted

### Webhooks > Kudos

- `POST   /webhooks/workvivo` — kudos.created
- `POST   /webhooks/workvivo` — kudos.updated
- `POST   /webhooks/workvivo` — kudos.published
- `POST   /webhooks/workvivo` — kudos.deleted

### Webhooks > Pages

- `POST   /webhooks/workvivo` — page.created
- `POST   /webhooks/workvivo` — page.updated
- `POST   /webhooks/workvivo` — page.deleted
- `POST   /webhooks/workvivo` — page.published

### Webhooks > Updates

- `POST   /webhooks/workvivo` — update.created
- `POST   /webhooks/workvivo` — update.updated
- `POST   /webhooks/workvivo` — update.published
- `POST   /webhooks/workvivo` — update.deleted


