from clientFactory import ClientFactory
from config import Config


def main():
    azure_config = Config("key", "endpoint")
    client_factory = ClientFactory(azure_config)
    client = client_factory.authenticateClient()

    try:
        documents = [
            {"id": "1", "language": "en",
             "text": "The kiss, dear maid! thy lip has left  Shall never part from mine Till happier hourse restore "
                     "the glift Untainted back to thine."},
            {"id": "2", "language": "en", "text": "Fill the goblet again! for I never before Felt the glow which now "
                                                  "gladdens my heart to its core;"},
            {"id": "3", "language": "ru", "text": "Поздравляем вам, Штирлиц! У вас родился сын! По щеке Штирлица "
                                                  "прокатилась слеза. Ону уже 5 лет не был дома..."}
        ]

        response = client.sentiment(documents=documents)
        for doc in response.documents:
            print("Doc ID: ", doc.id, ", Sentiment Score: ", "{:.2f}".format(doc.score))
    except Exception as err:
        print("Encountered exception. {}".format(err))


if __name__ == "__main__":
    main()
