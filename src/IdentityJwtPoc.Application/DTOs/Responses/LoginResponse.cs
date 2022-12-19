using System.Text.Json.Serialization;

namespace IdentityJwtPoc.Application.DTOs.Responses
{
    public class LoginResponse
    {
        public bool Sucesso => Errors.Count == 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AccessToken { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RefreshToken { get; private set; }

        public List<string> Errors { get; private set; } = new List<string>();

        public LoginResponse(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public void AddError(string erro) =>
            Errors.Add(erro);

        public void AddErrors(IEnumerable<string> erros) =>
            Errors.AddRange(erros);
    }
}
