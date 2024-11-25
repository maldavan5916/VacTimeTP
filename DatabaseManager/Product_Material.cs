namespace DatabaseManager
{
    public class Product_Material : BaseModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private int _productId;
        public int ProductId
        {
            get => _productId;
            set
            {
                _productId = value;
                OnPropertyChanged(nameof(ProductId));
            }
        }

        private Product? _product;
        public Product? Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        private int _materialId;
        public int MaterialId
        {
            get => _materialId;
            set
            {
                _materialId = value;
                OnPropertyChanged(nameof(MaterialId));
            }
        }

        private Material? _material;
        public Material? Material
        {
            get => _material;
            set
            {
                _material = value;
                OnPropertyChanged(nameof(Material));
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value > 0 ? value : 0;
                OnPropertyChanged(nameof(Quantity));
            }
        }
    }
}
