namespace Assets.Scripts.Shared
{
    public interface IStationStateSwitcher
    {
        public void SwitchState<T>() where T : BaseState;
    }
}