namespace Mylearningapi.Services
{
    public class TransientService
    {
        public Guid Id {get;}
        public TransientService()
        {
            // Guid.NewGuid(); is a built‑in .NET method that generates a brand‑new 
            // globally unique identifier (GUID). Think of it as a 128‑bit random ID 
            // that’s almost guaranteed to be unique across space and time.
            Id = Guid.NewGuid();
        }
    }
    public class ScopedService
    {
        public Guid Id {get;}
        public ScopedService()
        {
            Id=Guid.NewGuid();
        }
    }
    public class SingletonService
    {
        public Guid Id {get;} = Guid.NewGuid();
    }
}