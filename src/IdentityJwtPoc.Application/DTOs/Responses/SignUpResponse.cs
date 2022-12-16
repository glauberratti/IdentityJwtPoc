namespace IdentityJwtPoc.Application.DTOs.Responses
{
    public class SignUpResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; private set; }

        public SignUpResponse(bool success = true)
        {
            Success = success;
            Errors = new List<string>();
        }

        public void AddError(string erro) =>
            Errors.Add(erro);

        public void AddErrors(IEnumerable<string> erros) =>
            Errors.AddRange(erros);
    }
}
