from azure.cognitiveservices.language.textanalytics import TextAnalyticsClient
from msrest.authentication import CognitiveServicesCredentials


class ClientFactory:
    def __init__(self, config):
        self.config = config

    def authenticateClient(self):
        credentials = CognitiveServicesCredentials(self.config.key)
        client = TextAnalyticsClient(endpoint=self.config.endpoint, credentials=credentials)
        return client
