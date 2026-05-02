namespace Extensions;

public static class DateTimeExtensions
{
    extension(DateTime dateTime)
    {
        public DateTime RoundUp(TimeSpan timeSpan)
        {
            var modTicks = dateTime.Ticks % timeSpan.Ticks;
        
            var delta = modTicks != 0 ? timeSpan.Ticks - modTicks : 0;
        
            return new DateTime(dateTime.Ticks + delta, dateTime.Kind);
        }

        public DateTime RoundDown(TimeSpan timeSpan)
        {
            var delta = dateTime.Ticks % timeSpan.Ticks;
        
            return new DateTime(dateTime.Ticks - delta, dateTime.Kind);
        }
    }
}