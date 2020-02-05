namespace MyRokuSkill.Models
{
    public class Roku
    {
        public string LaunchApplication { get; set; }
        public int VolumeLevel { get; set; }
        public CommandType Command { get; set; }
        public ButtonType Button { get; set; }
        public RoomType Room { get; set; }


        public enum ButtonType
        {
            Up=1,
            Down=2,
            Left=3,
            Right=4,
            Select=5,
            Power=6,
            Play=7,
            FastForward=8,
            Rewind=9,
            Home=10,
            Back=11,
            Mute=12
        }

        public enum CommandType
        {
            KeyPress=1,
            Launch=2,
            Volume=3
        }

        public enum RoomType
        {
            LivingRoom=1,
            Bedroom=2
        }

        public Roku(string launchApp, RoomType room)
        {
            LaunchApplication = launchApp;
            Command = CommandType.Launch;
            Room = room;
        }

        public Roku(int volume, RoomType room)
        {
            Command = CommandType.Volume;
            VolumeLevel = volume;
            Room = room;
        }

        public Roku(ButtonType button, RoomType room)
        {
            Button = button;
            Command = CommandType.KeyPress;
            Room = room;
        }
    }
}
