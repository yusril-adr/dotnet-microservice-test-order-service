namespace DotnetOrderService.Immutables {
    public class FaultTolerant
    {
        public class Incident { 
            public int CurrentCount { get; set; } = 0;
            public DateTime ExpiredAt { get; set; } = DateTime.Now;
        }

        public static Dictionary<string, Incident> Incidents = new Dictionary<string, Incident>();

        public static void Set(string key, int cooldown) {
            var incident = Incidents.Where(x => x.Key == key).First().Value;
            var currentCount = incident?.CurrentCount ?? 0;

            if (incident != null) {
                Incidents.Remove(key);
            }
            
            Incidents.Add(key, new() {
                CurrentCount = currentCount + 1,
                ExpiredAt = DateTime.Now.AddMinutes(cooldown)
            });
        }
    } 
}
