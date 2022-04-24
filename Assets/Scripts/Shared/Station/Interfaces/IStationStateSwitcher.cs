namespace Assets.Scripts.Shared
{
    public interface IStationStateSwitcher
    {
        public void SwitchState<T>() where T : BaseState;
        public bool HasToUpdateOktmmf();
        public void SetHasToUpdateOktmmf(bool enabled);
    }
}