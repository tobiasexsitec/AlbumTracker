# Copilot Instructions

## Project Guidelines
- User prefers using GitHub repository variables (e.g., vars.API_BASE_URL) for configuration like API base URLs rather than looking them up dynamically via Azure CLI in workflows.
- Never commit or push `wwwroot/appsettings.json` with the Api BaseUrl set to localhost. It should always point to the production Azure Function App URL (https://func-albumtracker-umnowefiofffq.azurewebsites.net/) when committed.
- When committing and pushing, skip `wwwroot/appsettings.json` completely - do not stage or include it in commits.