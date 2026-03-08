@description('Azure region for all resources')
param location string

@description('Base name used for generating resource names')
param appName string

@secure()
@description('Spotify API Client ID')
param spotifyClientId string

@secure()
@description('Spotify API Client Secret')
param spotifyClientSecret string

@description('Email address for budget alerts')
param alertEmail string

@description('Budget start date (defaults to first of current month)')
param budgetStartDate string = utcNow('yyyy-MM-01')

// Unique suffix to avoid naming collisions on globally-unique resources
var uniqueSuffix = uniqueString(resourceGroup().id)

// --- Log Analytics Workspace ---
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'log-${appName}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    workspaceCapping: {
      dailyQuotaGb: 1
    }
  }
}

// --- Application Insights ---
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-${appName}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

// --- Storage Account (required by Azure Functions) ---
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: take('st${appName}${uniqueSuffix}', 24)
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

// --- App Service Plan (Consumption / Y1 — cheapest option) ---
resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: 'asp-${appName}'
  location: location
  kind: 'linux'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {
    reserved: true // required for Linux
  }
}

// --- Function App ---
resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: 'func-${appName}-${uniqueSuffix}'
  location: location
  kind: 'functionapp,linux'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    dailyMemoryTimeQuota: 50000
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower('func-${appName}')
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'WEBSITE_MAX_DYNAMIC_APPLICATION_SCALE_OUT'
          value: '1'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'Spotify__ClientId'
          value: spotifyClientId
        }
        {
          name: 'Spotify__ClientSecret'
          value: spotifyClientSecret
        }
      ]
      cors: {
        allowedOrigins: [
          'https://tobiasexsitec.github.io'
        ]
      }
    }
  }
}

// --- Budget Alert ---
resource budget 'Microsoft.Consumption/budgets@2023-11-01' = {
  name: 'budget-${appName}'
  properties: {
    timePeriod: {
      startDate: budgetStartDate
    }
    timeGrain: 'Monthly'
    amount: 5
    category: 'Cost'
    notifications: {
      actual80Percent: {
        enabled: true
        operator: 'GreaterThanOrEqualTo'
        threshold: 80
        contactEmails: [
          alertEmail
        ]
      }
      actual100Percent: {
        enabled: true
        operator: 'GreaterThanOrEqualTo'
        threshold: 100
        contactEmails: [
          alertEmail
        ]
      }
    }
  }
}

output functionAppName string = functionApp.name
output functionAppHostname string = functionApp.properties.defaultHostName
