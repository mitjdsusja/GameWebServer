namespace WebServerProject.Models.DTOs
{
    public record PlayerDto
    (
        string userId,
        string nickname,
        int level,
        int gold,
        int diamonds,
        int profileId,
        bool tutorialCompleted,
        DateTime createdAt
    );
}
