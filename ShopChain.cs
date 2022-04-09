using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


public static class Program
{

    public static void Main(string[] args)
    {
    
      CornerShop corner = new CornerShop("Fix price");
      DrugStore drugStore = new DrugStore("Drug store");
      Parking parking = new Parking("Parking");

      corner.AddProduct(new FoodStuff("Milk",0.85f, 4));

      drugStore.AddProduct(new Medicine("Red pills",0.5f, 12345, 3));

      parking.AddProduct(new ParkingTicket("Parking",0.2f, 121232));

      Customer customer = new Customer("Thomas", "Anderson", "88005553535");

      StoreChain chain = new StoreChain(customer);


      chain.AddShop(corner);
      chain.AddShop(drugStore);
      chain.AddShop(parking);

      chain.BuyProduct("Milk", 2);

      chain.BuyProduct("Red pills", 1);

      chain.BuyProduct("Parking", 1);

      chain.ApplyPurchaises();

      Console.WriteLine(chain.BillsReport());

      Console.ReadKey();
    }
}



public class StoreChain
{

  private Customer _customer;
  private List<Shop> _shops;

  private Dictionary<Shop,List<Purchaise>> _purchaises;

  private List<Bill> _bills;

  public StoreChain(Customer customer)
  {
     _customer = customer;
  }

  public void AddShop(Shop shop)
  {
      if(_shops == null)
        _shops = new List<Shop>();

      _shops.Add(shop);
  }

  public void RemoveShop(Shop shop)
  {
      if(_shops == null)
          return;

    _shops.Remove(shop);
  }
  
  public void BuyProduct(string name, int quantity)
  {

    if(_shops == null || _shops.Count == 0)
      return;


     Shop shop = FindShopByProduct(name, quantity);

     if(shop == null)
        return;

     Purchaise purchaise = shop.BuyProduct(name,quantity);    

     if(purchaise == null)
       return;

       if(_purchaises == null)
          _purchaises = new Dictionary<Shop,List<Purchaise>>();

      if(_purchaises.ContainsKey(shop))
      {
        _purchaises[shop].Add(purchaise);
      }
      else
      {
        _purchaises.Add(shop, new List<Purchaise>(){purchaise});
      }
  }

  public void ApplyPurchaises()
  {
      GenerateBills();

      _shops.Clear();
      _purchaises.Clear();
  }

  private void GenerateBills()
  {
    _bills = new List<Bill>();

    foreach(Shop shop in _purchaises.Keys)
    {
      Bill bill = new Bill(_customer,shop,_purchaises[shop]);
      _bills.Add(bill);

    }
  }

  public string BillsReport()
  {
     StringBuilder builder = new StringBuilder();

     foreach(Bill bill in _bills)
     {
       builder.Append(bill.ToString());
       builder.Append(Environment.NewLine);
     }

     return builder.ToString();
  }

  private Shop FindShopByProduct(string name, int quantity)
  {
     return _shops.FirstOrDefault(_Shop => _Shop.HasProduct(name));
  }
}

public class Purchaise
{
    #region  construction
    public Purchaise(Product product, int quantity = 0)
    {
      _product = product;
      _quantity = quantity;
    }
    #endregion

    #region  properties
    public Product Product {get{ return _product;}}
    public int Quantity {get{return _quantity;}}
    #endregion

    #region  atributes
    private Product _product;
    private int _quantity;
    #endregion


}

public class Bill
{
   private readonly Customer _customer;
   private readonly Shop _shop;
   private readonly List<Purchaise> _purchaises;

   public Bill(Customer customer, Shop shop, List<Purchaise> purchaises)
   {
     _customer = customer;
     _shop = shop;
     _purchaises = purchaises; 
   }

   public override string ToString()
   {
      StringBuilder billString = new StringBuilder();

      billString.Append(_shop.Name + " " + "Type : " + _shop.GetType().ToString());

      billString.Append(Environment.NewLine);

      foreach(Purchaise purchaise in _purchaises)
      {
        billString.Append(purchaise.Product.ToString());
        billString.Append(Environment.NewLine);
      }

      billString.Append(_customer.Name);
      billString.Append(Environment.NewLine);
      billString.Append(_customer.LastName);
      billString.Append(Environment.NewLine);
      billString.Append(_customer.PhoneNumber);
      billString.Append(Environment.NewLine);

      return billString.ToString();

   }


}

public abstract class Shop 
{
  #region  properties

  public string Name {get{return _name;}}

  #endregion


  #region atributes 
  protected string _name;
  protected List<Product> _products;
  #endregion

  #region  abstract

  public abstract Purchaise BuyProduct(string name, int quantity);
  public abstract Product GetProduct(string name, int quantity);

  #endregion

  public void AddProduct(Product product)
  {
    if(_products == null)
        _products = new List<Product>();

    _products.Add(product);
  }

  public bool HasProduct(string name)
  {
      if(_products == null)
        return false;

      return _products.Any(_Product => _Product != null && _Product.Name == name);
  }

    
}

public class DrugStore : Shop
{
  public DrugStore(string name)
  {
    _name = name;
  }

  public override Purchaise BuyProduct(string name, int quantity)
  {
      Medicine product = GetProduct(name, quantity) as Medicine;
      Purchaise purchaised = CreatePurchaisedProduct(product, quantity);

      UpdateProductQuantity(product, product.Quantity - quantity);

      return purchaised;
  }

  private Purchaise CreatePurchaisedProduct(Medicine product, int quantity)
  {
     Purchaise purchaised = new Purchaise(product, quantity);

     return purchaised;
  }

  private void UpdateProductQuantity(Medicine product, int  quantity)
  {
      if(quantity <= 0)
        _products.Remove(product);

      product.Quantity = quantity;    
  }

  public override Product GetProduct(string name, int quantity)
  {
      Medicine product = _products.FirstOrDefault(_Product => _Product != null && _Product.Name == name) as Medicine;

      if(product.Quantity < quantity)
        return null;

      if(product == null)
        return null;

      return product;
  }
}

public class CornerShop : Shop
{

  public CornerShop(string name)
  {
    _name = name;
  }

  public override Purchaise BuyProduct(string name, int quantity)
  {
      FoodStuff product = GetProduct(name, quantity) as FoodStuff;
      Purchaise purchaised = CreatePurchaisedProduct(product, quantity);

      UpdateProductQuantity(product, product.Quantity - quantity);

      return purchaised;
  }

  private Purchaise CreatePurchaisedProduct(FoodStuff product, int quantity)
  {
    Purchaise purchaised = new Purchaise(product,quantity);

    return purchaised;
  }

  private void UpdateProductQuantity(FoodStuff product, int  quantity)
  {
      if(quantity <= 0)
        _products.Remove(product);

      product.Quantity = quantity;    
  }

  public override Product GetProduct(string name, int quantity)
  {
      FoodStuff product = _products.FirstOrDefault(_Product => _Product != null && _Product.Name == name) as FoodStuff;

      

      if(product == null && product.Quantity < quantity)
        return null;

      return product;
  }

}

public class Parking : Shop
{

  public Parking(string name)
  {
    _name = name;
  }
  

  public override Purchaise BuyProduct(string name, int quantity)
  {
      ParkingTicket product = GetProduct(name, quantity) as ParkingTicket;
      Purchaise purchaised = CreatePurchaisedProduct(product, quantity);

      return purchaised;
  }

  public override Product GetProduct(string name, int quantity)
  {
      Product product = _products.FirstOrDefault(_Product => _Product != null && _Product.Name == name);

      if(product == null)
        return null;

      return product;
  }

  private Purchaise CreatePurchaisedProduct(ParkingTicket product, int quantity)
  {
    Purchaise purchaised = new Purchaise(product,quantity);

    return purchaised;
  }
}


public class Product
{
    #region  construction

    public Product(string name, float price)
    {
      _name = name;
      _price = price;
    }
    
    #endregion

    #region  properties
  
    public string Name {get {return _name;}}

    #endregion
    
    #region  atributes

    protected string _name;
    protected float _price;

    #endregion

    #region  public 

    public override string ToString()
    {
      return _name;
    }

    #endregion

}

public class FoodStuff : Product
{
    #region  construction

    public FoodStuff(string name, float price, int quantity = 0)
      :base(name,price)
    {
      Quantity = quantity;
    }
    
    #endregion

    #region  properties

    public int Quantity 
    {
      get 
      {
        return _quantity;
      }
      set 
      {
          if(value < 0)
            value = 0;

          _quantity = value;
      }
    }

    #endregion

    #region  atributes

    protected int _quantity;

    #endregion

    #region  public 

    public override string ToString()
    {
      return _name + " " + " " + (_price * _quantity).ToString();
    }

    #endregion

}

public class ParkingTicket : Product
{
    #region  construction

    public ParkingTicket(string name, float price, int serial)
    :base(name, price)
    {
      _serial = serial;
    }

    #endregion

    #region  properties

    public int Hours 
    {
      get 
      {
        return _hours;
      }
      set 
      {
          if(value < 1)
            value = 1;

          _hours = value;
      }
    }

    #endregion

    #region  atributes

    private int _hours;
    private int _serial;
    #endregion

    #region  public 

    public override string ToString()
    {
      return  _name + " " +  _serial +  " " + ( _price * (1 + _hours)).ToString();
    }

    #endregion
}

public class Medicine : Product
{
    #region  construction

    public Medicine(string name, float price, int serial, int quantity = 0)
    :base(name, price)
    {
      _serial = serial;
      Quantity = quantity;
    }

    #endregion

    
    #region  properties

    public int Quantity 
    {
      get 
      {
        return _quantity;
      }
      set 
      {
          if(value < 0)
            value = 0;

          _quantity = value;
      }
    }

    #endregion

    #region atributes
    
    private int _serial;
     private int _quantity;

    #endregion

    #region  public 

    public override string ToString()
    {
      return _name + " " + _serial.ToString() + " " + (_price * _quantity).ToString();
    }

    #endregion

}



public class Customer
{

  #region  construction    
  
  public Customer(string name, string lastName, string phoneNumber)
  {
    _name = name;
    _lastName = lastName;
    _phoneNumber = phoneNumber;
  }

  #endregion

  #region  properties

  public string Name {get { return _name;}}
  public string LastName {get {return _lastName;}}
  public string PhoneNumber {get{return _phoneNumber;}}
  #endregion

  #region atributes

  private readonly string _name;
  private readonly string _lastName;
  private readonly string _phoneNumber;

  #endregion

}
