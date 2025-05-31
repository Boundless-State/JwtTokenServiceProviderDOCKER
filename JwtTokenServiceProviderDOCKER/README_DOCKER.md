# JwtTokenServiceProvider

A lightweight Dockerized microservice REST API to generate and verify JWT tokens.

---

## 🐳 Run with Docker

1. Open terminal in the root of the project (where the `Dockerfile` is)
2. Build the image:

```bash
docker build -t jwt-token-service .
```

3. Run the container:

```bash
docker run -d -p 5001:8080 --name jwt-token-service jwt-token-service
```

4. Open Swagger UI in your browser:

```
http://localhost:5001/swagger
```

---

## 🛡️ Endpoints

### 🔑 POST `/api/auth/token`

Generates a JWT token based on user ID and role.

#### Request Body

```json
{
  "userId": "testuser",
  "role": "Admin"
}
```

#### Response

```json
{
  "token": "generated token"
}
```

---

### 🔍 POST `/api/auth/verify`

Verifies a JWT token and returns decoded information if valid.

#### Request Body

```json
{
  "token": "generated token"
}
```

#### Response (valid token)

```json
{
  "validToken": true,
  "userId": "testuser",
  "role": "Admin"
}
```

#### Response (invalid token)

```json
{
  "validToken": false,
  "error": "SecurityTokenExpiredException: ..."
}
```

---

## 🧪 Example: C# Client Call to Generate Token

```csharp
using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5001");

var loginPayload = new
{
    userId = "testuser",
    role = "Admin"
};

var response = await client.PostAsJsonAsync("/api/auth/token", loginPayload);
var content = await response.Content.ReadFromJsonAsync<TokenResponse>();

Console.WriteLine("JWT Token: " + content.token);
```

---

## ✅ Example: C# Client Call to Verify Token

```csharp
using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5001");

// Token retrieved from login
var token = "{tokenvalue}...";

var verifyPayload = new
{
    token = token
};

var response = await client.PostAsJsonAsync("/api/auth/verify", verifyPayload);
var result = await response.Content.ReadFromJsonAsync<TokenValidationResult>();

if (result.validToken)
{
    Console.WriteLine($"User ID: {result.userId}, Role: {result.role}");
}
else
{
    Console.WriteLine($"Invalid token: {result.error}");
}
```

---

## 📜 How to use programmatically:

- Add a `TokensClient.cs` file to your project
- Use the `TokensClient` class to interact with this microservice
- Example implementation:

```csharp
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JwtTokenClient;

public class TokenResponse
{
    public string Token { get; set; }
}

public class TokenValidationResult
{
    public bool ValidToken { get; set; }
    public string UserId { get; set; }
    public string Role { get; set; }
    public string Error { get; set; }
}

public class TokensClient
{
    private readonly HttpClient _httpClient;

    public TokensClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenerateTokenAsync(string userId, string role)
    {
        var payload = new
        {
            userId,
            role
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/token", payload);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result.Token;
    }

    public async Task<TokenValidationResult> VerifyTokenAsync(string token)
    {
        var payload = new
        {
            token
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/verify", payload);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TokenValidationResult>();
    }
}
```

---

## 📦 NuGet Packages Used

- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Swashbuckle.AspNetCore`
- `Swashbuckle.AspNetCore.Annotations`