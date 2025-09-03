namespace Ambev.DeveloperEvaluation.Common.Security
{
    /// <summary>
    /// Abstração para hash e verificação de senha.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Gera o hash da senha para armazenamento.
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifica se a senha em texto puro corresponde ao hash armazenado.
        /// </summary>
        bool Verify(string password, string passwordHash);
    }
}
