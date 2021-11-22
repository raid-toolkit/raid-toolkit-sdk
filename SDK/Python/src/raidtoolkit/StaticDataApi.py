from .RemoteApiClient import RemoteApiClient


class StaticDataApi:
    def __init__(self, client: RemoteApiClient):
        self.client = client

    def get_all_data(self):
        return self.client.call("static-data", "getAllData")

    def get_localized_strings(self):
        return self.client.call("static-data", "getLocalizedStrings")

    def get_arena_data(self):
        return self.client.call("static-data", "getArenaData")

    def get_artifact_data(self):
        return self.client.call("static-data", "getArtifactData")

    def get_hero_data(self):
        return self.client.call("static-data", "getHeroData")

    def get_skill_data(self):
        return self.client.call("static-data", "getSkillData")

    def get_stage_data(self):
        return self.client.call("static-data", "getStageData")
