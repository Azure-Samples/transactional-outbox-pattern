param location string = resourceGroup().location

// Cosmos DB Account
resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2025-04-15' = {
  name: 'cosmos-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: true
    publicNetworkAccess: 'Enabled'
  }
}

// Cosmos DB
resource cosmosDbDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-04-15' = {
  parent: cosmosDbAccount
  name: 'tobp'
  location: location
  properties: {
    resource: {
      id: 'tobp'
    }
    options: {
      autoscaleSettings: {
        maxThroughput: 4000
      }
    }
  }
}

// Data Container
resource containerData 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-04-15' = {
  parent: cosmosDbDatabase
  name: 'data'
  location: location
  properties: {
    resource: {
      id: 'data'
      partitionKey: {
        paths: [
          '/partitionKey'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        automatic: true
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      defaultTtl: -1
    }
  }
}

// Leases Container
resource containerLeases 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-04-15' = {
  parent: cosmosDbDatabase
  name: 'leases'
  location: location
  properties: {
    resource: {
      id: 'leases'
      partitionKey: {
        paths: [
          '/id'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        automatic: true
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }
  }
}

// ServiceBus
resource sb 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: 'sb-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  // Contacts Topic
  resource sbtContacts 'topics@2021-11-01' = {
    name: 'sbt-contacts'
    properties: {
      defaultMessageTimeToLive: 'P2D'
      maxSizeInMegabytes: 1024
      requiresDuplicateDetection: false
      duplicateDetectionHistoryTimeWindow: 'PT10M'
      enableBatchedOperations: false
      supportOrdering: false
      autoDeleteOnIdle: 'P2D'
      enablePartitioning: true
      enableExpress: false
    }

    resource sbtTestSubscription 'subscriptions@2024-01-01' = {
      name: 'testSubscription'
      properties: {
        lockDuration: 'PT5M'
        requiresSession: true
        defaultMessageTimeToLive: 'P2D'
        deadLetteringOnMessageExpiration: true
        maxDeliveryCount: 10
        enableBatchedOperations: false
        autoDeleteOnIdle: 'P2D'
      }
    }
  }
}


// ApplicationInsights
resource workspace 'Microsoft.OperationalInsights/workspaces@2025-02-01' = {
  name: 'ws-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource appi 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
  }
}

// Storage Account for Azure Function
resource storageAccount 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: 'st${replace(uniqueString(resourceGroup().id), '-', '')}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

// App Service Plan for Azure Function
resource functionAppServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: 'asp-func-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {
    reserved: false
  }
}

// Azure Function App
resource functionApp 'Microsoft.Web/sites@2024-11-01' = {
  name: 'func-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: functionAppServicePlan.id
    siteConfig: {
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
          value: toLower('func-tobp-${uniqueString(resourceGroup().id)}')
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
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appi.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appi.properties.ConnectionString
        }
        {
          name: 'Cosmos:Url'
          value: 'https://${cosmosDbAccount.name}.documents.azure.com:443/'
        }
        {
          name: 'Cosmos:Key'
          value: listKeys('${cosmosDbAccount.id}', cosmosDbAccount.apiVersion).primaryMasterKey
        }
        {
          name: 'Cosmos:Db'
          value: 'tobp'
        }
        {
          name: 'Cosmos:Container'
          value: 'data'
        }
        {
          name: 'Events:Ttl'
          value: '864000'
        }
        {
          name: 'ServiceBus:ConnectionString'
          value: listKeys('${sb.id}/AuthorizationRules/RootManageSharedAccessKey', sb.apiVersion).primaryConnectionString
        }
      ]
      netFrameworkVersion: 'v8.0'
      use32BitWorkerProcess: false
      cors: {
        allowedOrigins: [
          'https://portal.azure.com'
        ]
      }
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: 'appservice-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'S1'
    capacity: 1
  }
  properties: {}
}

resource appService 'Microsoft.Web/sites@2024-11-01' = {
  name: 'webapp-tobp-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: false
    siteConfig: {
      alwaysOn: true
      use32BitWorkerProcess: false
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
      appSettings: [
        {
          name: 'Cosmos:Url'
          value: 'https://${cosmosDbAccount.name}.documents.azure.com:443/'
        }
        {
          name: 'Cosmos:Key'
          value: listKeys('${cosmosDbAccount.id}', cosmosDbAccount.apiVersion).primaryMasterKey
        }
        {
          name: 'Cosmos:Db'
          value: 'tobp'
        }
        {
          name: 'Cosmos:Container'
          value: 'data'
        }
        {
          name: 'Events:Ttl'
          value: '864000'
        }
      ]
    }
  }
}

output sbTopicName string = 'sbt-contacts'
output cosmosUri string = 'https://${cosmosDbAccount.name}.documents.azure.com:443/'
output cosmosDbName string = 'tobp'
output cosmosDbDataContainerName string = 'data'
output cosmosDbLeasesContainerName string = 'leases'
output functionAppName string = functionApp.name
