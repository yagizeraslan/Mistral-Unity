namespace YagizEraslan.Mistral.Unity
{
    public enum MistralModel
    {
        Mistral_Large_Latest,
        Mistral_Medium_Latest,
        Mistral_Small_Latest,
        Codestral_Latest,
        Ministral_8B_Latest,
        Ministral_3B_Latest,
        Open_Mistral_Nemo,
        Pixtral_Large_Latest
    }

    public static class MistralModelExtensions
    {
        public static string ToModelString(this MistralModel model)
        {
            return model switch
            {
                MistralModel.Mistral_Large_Latest => "mistral-large-latest",
                MistralModel.Mistral_Medium_Latest => "mistral-medium-latest",
                MistralModel.Mistral_Small_Latest => "mistral-small-latest",
                MistralModel.Codestral_Latest => "codestral-latest",
                MistralModel.Ministral_8B_Latest => "ministral-8b-latest",
                MistralModel.Ministral_3B_Latest => "ministral-3b-latest",
                MistralModel.Open_Mistral_Nemo => "open-mistral-nemo",
                MistralModel.Pixtral_Large_Latest => "pixtral-large-latest",
                _ => "mistral-small-latest"
            };
        }
    }
}
