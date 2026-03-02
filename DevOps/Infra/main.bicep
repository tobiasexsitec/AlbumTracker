targetScope = 'subscription'

@description('Azure region for all resources')
param location string = 'swedencentral'

@description('Base name used for generating resource names')
param appName string = 'albumtracker'

@secure()
@description('Spotify API Client ID')
param spotifyClientId string

@secure()
@description('Spotify API Client Secret')
param spotifyClientSecret string

var resourceGroupName = 'rg-${appName}'

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
}

module resources 'resources.bicep' = {
  scope: rg
  params: {
    location: location
    appName: appName
    spotifyClientId: spotifyClientId
    spotifyClientSecret: spotifyClientSecret
  }
}

output functionAppName string = resources.outputs.functionAppName
output functionAppHostname string = resources.outputs.functionAppHostname
