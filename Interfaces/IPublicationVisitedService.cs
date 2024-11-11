public interface IPublicationVisitedService
{
    public string getLatestCategoryVisitedByUser(int userId);
    public List<PublicationVisitedDTO> GetLastTenVisitedPublications(int userId);
    public void AddPublicationVisit(int userId, int publicationId);
}
