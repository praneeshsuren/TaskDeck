# TaskDeck API Contract

## Base URL

```
Development: http://localhost:5000
Production: https://api.taskdeck.com
```

## Authentication

All endpoints except public ones require a JWT token in the Authorization header:

```
Authorization: Bearer <token>
```

## Endpoints

### Health Check

```http
GET /health
```

Response: `200 OK`

---

## Authentication

### Login with Firebase

```http
POST /api/auth/firebase
Content-Type: application/json

{
  "idToken": "string"
}
```

Response:
```json
{
  "token": "string",
  "expiresAt": "2024-01-01T00:00:00Z",
  "user": {
    "id": "guid",
    "email": "string",
    "displayName": "string",
    "avatarUrl": "string"
  }
}
```

---

## Projects

### List Projects

```http
GET /api/projects
```

Response:
```json
[
  {
    "id": "guid",
    "name": "string",
    "description": "string",
    "color": "string",
    "icon": "string",
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

### Create Project

```http
POST /api/projects
Content-Type: application/json

{
  "name": "string",
  "description": "string",
  "color": "#3b82f6",
  "icon": "folder"
}
```

### Get Project

```http
GET /api/projects/{id}
```

### Update Project

```http
PUT /api/projects/{id}
Content-Type: application/json

{
  "name": "string",
  "description": "string",
  "color": "string",
  "icon": "string"
}
```

### Delete Project

```http
DELETE /api/projects/{id}
```

---

## Tasks

### List Tasks by Project

```http
GET /api/projects/{projectId}/tasks
```

Response:
```json
[
  {
    "id": "guid",
    "title": "string",
    "description": "string",
    "status": "Todo|InProgress|InReview|Done|Cancelled",
    "priority": "Low|Medium|High|Urgent",
    "order": 0,
    "dueDate": "2024-01-01T00:00:00Z",
    "completedAt": null,
    "createdAt": "2024-01-01T00:00:00Z",
    "projectId": "guid",
    "assignedTo": {
      "id": "guid",
      "displayName": "string",
      "avatarUrl": "string"
    },
    "createdBy": {
      "id": "guid",
      "displayName": "string"
    }
  }
]
```

### Create Task

```http
POST /api/projects/{projectId}/tasks
Content-Type: application/json

{
  "title": "string",
  "description": "string",
  "priority": "Medium",
  "dueDate": "2024-01-01T00:00:00Z",
  "assignedToId": "guid"
}
```

### Update Task

```http
PUT /api/tasks/{id}
Content-Type: application/json

{
  "title": "string",
  "description": "string",
  "status": "InProgress",
  "priority": "High",
  "dueDate": "2024-01-01T00:00:00Z",
  "assignedToId": "guid"
}
```

### Delete Task

```http
DELETE /api/tasks/{id}
```

### Reorder Tasks

```http
PUT /api/tasks/reorder
Content-Type: application/json

{
  "projectId": "guid",
  "taskOrders": [
    { "taskId": "guid", "order": 0 },
    { "taskId": "guid", "order": 1 }
  ]
}
```

---

## Users

### Get Current User

```http
GET /api/users/me
```

### Get User by ID

```http
GET /api/users/{id}
```

### Update Profile

```http
PUT /api/users/me
Content-Type: application/json

{
  "displayName": "string",
  "avatarUrl": "string"
}
```

---

## SignalR Hubs

### Tasks Hub

**Connection URL:** `/hubs/tasks`

#### Client → Server Events

| Event | Payload | Description |
|-------|---------|-------------|
| `JoinProject` | `projectId: string` | Subscribe to project updates |
| `LeaveProject` | `projectId: string` | Unsubscribe from project updates |

#### Server → Client Events

| Event | Payload | Description |
|-------|---------|-------------|
| `TaskCreated` | `TaskDto` | New task created |
| `TaskUpdated` | `TaskDto` | Task was updated |
| `TaskDeleted` | `taskId: string` | Task was deleted |

---

## Error Responses

All errors follow this format:

```json
{
  "code": "ERROR_CODE",
  "message": "Human readable message",
  "traceId": "string",
  "errors": {
    "fieldName": ["error message"]
  }
}
```

### Error Codes

| Status | Code | Description |
|--------|------|-------------|
| 400 | `VALIDATION_ERROR` | Request validation failed |
| 401 | `UNAUTHORIZED` | Authentication required |
| 403 | `FORBIDDEN` | Access denied |
| 404 | `NOT_FOUND` | Resource not found |
| 500 | `INTERNAL_ERROR` | Server error |
