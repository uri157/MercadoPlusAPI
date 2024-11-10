public interface IPublicationVisitedService
{
    public List<PublicationVisitedDTO> GetLastTenVisitedPublications(int userId);
    public void AddPublicationVisit(int userId, int publicationId);
}
