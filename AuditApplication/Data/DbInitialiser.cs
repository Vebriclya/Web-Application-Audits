namespace AuditApplication.Data;

public class DbInitialiser
{
    public static void Initialise(AuditContext context)
    {
        context.Database.EnsureCreated();
        
        // Check to see if the database already contains templates
        if (context.AuditTemplates.Any())
        {
            return; // Db has already been seeded 
        }
        
        // Any additional sample data can be added here 
    }
}