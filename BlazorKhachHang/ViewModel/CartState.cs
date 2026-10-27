namespace BlazorKhachHang.ViewModel
{
    // tạo class thông báo cho Layout khi giỏ hàng thay đổi.
    public class CartState
    {
        public int CartCount { get; private set; }

        public event Action? OnChange;

        public void SetCount(int count)
        {
            CartCount = count;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
