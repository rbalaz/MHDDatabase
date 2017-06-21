namespace MHDDatabase
{
    class Vehicle
    {
        public string vehicle { get; set; }
        public Types type { get; set; }
        public int amount { get; set; }

        public Vehicle(string vehicle, Types type)
        {
            this.vehicle = vehicle;
            this.type = type;
            amount = 0;
        }

        public Vehicle(string vehicle, Types type, int amount) : this(vehicle, type)
        {
            this.amount = amount;
        }

        public override string ToString()
        {
            return vehicle + " " + amount;
        }
    }
}
