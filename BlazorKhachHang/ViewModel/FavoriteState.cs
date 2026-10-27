namespace BlazorKhachHang.ViewModel
{
    // hiển thị yêu thích sản phẩm
    public class FavoriteState
    {
        public int FavoriteCount { get; private set; }

        public event Action? OnChange;

        public void SetCount(int count)
        {
            FavoriteCount = count;
            NotifyStateChanged();
        }

        public void Increase()
        {
            FavoriteCount++;
            NotifyStateChanged();
        }

        public void Decrease()
        {
            if (FavoriteCount > 0)
                FavoriteCount--;

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
