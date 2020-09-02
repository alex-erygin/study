from azure.cognitiveservices.language.textanalytics import TextAnalyticsClient
from msrest.authentication import CognitiveServicesCredentials

from config import Config


class ClientFactory:
    def __init__(self, config: Config):
        self.config = config

    def authenticateClient(self) -> TextAnalyticsClient:
        credentials = CognitiveServicesCredentials(self.config.key)
        client = TextAnalyticsClient(endpoint=self.config.endpoint, credentials=credentials)
        return client
