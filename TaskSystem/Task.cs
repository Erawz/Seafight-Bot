using System;

namespace BoxyBot.TaskSystem
{
    public class Task
    {
        public Task(string type, string id, int duration)
        {
            this.type = type;
            this.id = id;
            this.duration = duration;
        }

        public string type;
        public string id;
        public int duration;
        public DateTime startTime;
    }
}
