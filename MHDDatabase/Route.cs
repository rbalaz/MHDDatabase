namespace MHDDatabase
{
    class Route
    {
        public string route { get; set; }
        public Types type { get; set; }
        public int amount { get; set; }

        public Route(string route, Types type)
        {
            this.route = route;
            this.type = type;
            amount = 0;
        }

        public Route(string route, Types type, int amount) : this(route,type)
        {
            this.amount = amount;
        }

        public override string ToString()
        {
            return route + " " + amount;
        }
    }
}
