namespace SqlDataProvider.Data
{
    public class BallInfo
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Crater { get; set; }

        public int AttackResponse { get; set; }

        public double Power { get; set; }

        public int Radii { get; set; }

        public int Amount { get; set; }

        public string FlyingPartical { get; set; }

        public string BombPartical { get; set; }

        public bool IsSpin { get; set; }

        public int Mass { get; set; }

        public double SpinVA { get; set; }

        public int SpinV { get; set; }

        public int Wind { get; set; }

        public int DragIndex { get; set; }

        public int Weight { get; set; }

        public bool Shake { get; set; }

        public int Delay { get; set; }

        public string ShootSound { get; set; }

        public string BombSound { get; set; }

        public int ActionType { get; set; }

        public bool HasTunnel { get; set; }

        public bool IsSpecial()
        {
            int id = this.ID;
            if (id <= 59)
            {
                switch (id - 1)
                {
                    case 0:
                    case 2:
                    case 4:
                        break;
                    case 1:
                    case 3:
                        return false;
                    default:
                        if (id != 16 && id != 59)
                            return false;
                        else
                            break;
                }
            }
            else if (id != 64)
            {
                switch (id - 97)
                {
                    case 0:
                    case 1:
                    label_9:
                        break;
                    default:
                        switch (id - 10001)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                                goto label_9;
                            default:
                                return false;
                        }
                }
            }
            return true;
        }
    }
}
