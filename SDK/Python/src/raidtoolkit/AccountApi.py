from .RemoteApiClient import RemoteApiClient


class AccountApi:
    def __init__(self, client: RemoteApiClient):
        self.client = client

    def get_accounts(self):
        return self.client.call("account-api", "getAccounts")

    def get_account(self, accountId: str):
        return self.client.call("account-api", "accountInfo", [accountId])

    def get_account_dump(self, accountId: str):
        return self.client.call("account-api", "getAccountDump", [accountId])

    def get_all_resources(self, accountId: str):
        return self.client.call("account-api", "getAllResources", [accountId])

    def get_artifacts(self, accountId: str):
        return self.client.call("account-api", "getArtifacts", [accountId])

    def get_artifact_by_id(self, accountId: str, artifactId: str):
        return self.client.call("account-api", "getArtifactById", [accountId, artifactId])

    def get_heroes(self, accountId: str, snapshot: bool):
        return self.client.call("account-api", "getHeroes", [accountId, snapshot])

    def get_hero_by_id(self, accountId: str, heroId: str, snapshot: bool):
        return self.client.call("account-api", "getHeroById", [accountId, heroId, snapshot])

    def get_arena(self, accountId: str):
        return self.client.call("account-api", "getArena", [accountId])

    def get_academy(self, accountId: str):
        return self.client.call("account-api", "getAcademy", [accountId])
