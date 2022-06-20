using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Product_DelegateTraining
{
    public string ItemName = "Product name";
    public float ItemPrice = 0.0f;
    public uint Quantity = 1;

    public Product_DelegateTraining(string itemName, float itemPrice, uint quantity = 1)
    {
        ItemName = itemName;
        ItemPrice = itemPrice;
        Quantity = quantity;
    }
}

public sealed class Customer_DelegateTraining
{
    public void ShopGroceries(ShoppingCart_DelegateTraining shoppingCart)
    {
        shoppingCart.AddProduct(new Product_DelegateTraining("Aggourakia", 3.65f),
                                new Product_DelegateTraining("Spanashi", 2.57f),
                                new Product_DelegateTraining("Tomates", 4.28f),
                                new Product_DelegateTraining("Aggourakia", 3.65f));

        shoppingCart.RemoveProduct("Spanashi");

        shoppingCart.AddProduct(new Product_DelegateTraining("Repanaki", 5.42f, 2));
    }

    public void ShopMeat(ShoppingCart_DelegateTraining shoppingCart)
    {
        shoppingCart.AddProduct(new Product_DelegateTraining("Xirino", 7.35f),
                                new Product_DelegateTraining("Vodino", 12.57f));
    }

    public void ShopClothes(ShoppingCart_DelegateTraining shoppingCart)
    {
        shoppingCart.AddProduct(new Product_DelegateTraining("T-shirt", 12.34f),
                                new Product_DelegateTraining("Jeans", 20.87f),
                                new Product_DelegateTraining("Underwear", 17.83f));

        shoppingCart.RemoveProduct("Jeans");
    }

    public void ShopHygiene(ShoppingCart_DelegateTraining shoppingCart)
    {
        shoppingCart.AddProduct(new Product_DelegateTraining("ToothPaste", 2.15f),
                                new Product_DelegateTraining("Soap", 4.62f),
                                new Product_DelegateTraining("Towels", 18.80f));
    }

    public void ShopGuns(ShoppingCart_DelegateTraining shoppingCart)
    {
        shoppingCart.AddProduct(new Product_DelegateTraining("Ak47", 420.69f),
                                new Product_DelegateTraining("RPG", 1738.0f));
    }
}

public sealed class ShoppingCart_DelegateTraining
{
    private readonly Dictionary<string, Product_DelegateTraining> productsInCart = new();

    public void AddProduct(params Product_DelegateTraining[] product)
    {
        for (int i = 0; i < product.Length; i++)
        {
            if (productsInCart.TryGetValue(product[i].ItemName, out Product_DelegateTraining p))
            {
                p.Quantity += 1;
            }

            if (!productsInCart.ContainsKey(product[i].ItemName))
            {
                productsInCart.Add(product[i].ItemName, product[i]);
            }
        }
    }

    public void RemoveProduct(params string[] product)
    {
        for (int i = 0; i < product.Length; i++)
        {
            if (productsInCart.TryGetValue(product[i], out Product_DelegateTraining foundProduct))
            {
                foundProduct.Quantity -= 1;
                if (foundProduct.Quantity <= 0)
                    productsInCart.Remove(product[i]);
            }
        }
    }

    public Product_DelegateTraining[] GetProducts 
    { 
        get 
        { 
            return productsInCart.Values.ToArray(); 
        } 
    }

    public void DiscardCart()
    {
        Debug.Log("=================================");
        productsInCart.Clear();
    }
}

public sealed class Cashier_DelegateTraining
{
    private float Total { get; set; }
    
    /// <summary>
    ///     Customer will go through the full process of the shop. The cashier will calculate the total give him the receipt and "empty" the shopping cart.
    /// </summary>
    /// <param name="shoppingCart">The customer's shopping cart</param>
    /// <param name="calculateTotal">The function that calculates the total $ for the current state of the customer's shopping cart</param>
    /// <param name="getReceipt">The function that will print the receipt for the customer</param>
    public void CheckOut(ShoppingCart_DelegateTraining shoppingCart, Action<ShoppingCart_DelegateTraining, Action> calculateTotal, Action<ShoppingCart_DelegateTraining, Action> getReceipt)
    {
        calculateTotal(shoppingCart, () =>
        {
            getReceipt(shoppingCart, () =>
            {
                shoppingCart.DiscardCart();
            });
        });
    }

    /// <summary>
    ///     The function that calculates the total $ for the current state of the customer's shopping cart
    /// </summary>
    /// <param name="shoppingCart">List of products</param>
    public void CalculateTotal(ShoppingCart_DelegateTraining shoppingCart, Action onComplete = null)
    {
        Total = 0.0f;
        for (int i = 0; i < shoppingCart.GetProducts.Length; i++)
        {
            for (int j = 0; j < shoppingCart.GetProducts[i].Quantity; j++)
            {
                Total += shoppingCart.GetProducts[i].ItemPrice;
            }
        }

        onComplete?.Invoke();
    }

    /// <summary>
    ///     The function that will print the receipt for the customer
    /// </summary>
    /// <param name="shoppingCart">List of products</param>
    public void GiveReceipt(ShoppingCart_DelegateTraining shoppingCart, Action onComplete = null)
    {
        CalculateTotal(shoppingCart, () =>
        {
            string receipt = "Qty  | Item            | Price      | Total \n";
            for (int i = 0; i < shoppingCart.GetProducts.Length; i++)
            {
                receipt += "x" + shoppingCart.GetProducts[i].Quantity + "     |    " + 
                           shoppingCart.GetProducts[i].ItemName + "     |    " + 
                           shoppingCart.GetProducts[i].ItemPrice + "$     |    " + 
                           shoppingCart.GetProducts[i].Quantity * shoppingCart.GetProducts[i].ItemPrice + "$\n";
            }
            receipt += "---------------------------------\n" +
                       "Total before discount: " + Total + "$";
            
            Debug.LogFormat(receipt);
        });

        onComplete?.Invoke();
    }
}

public sealed class DelegatesTraining : MonoBehaviour
{
    private static readonly Customer_DelegateTraining customer = new();
    private static readonly ShoppingCart_DelegateTraining shoppingCart = new();
    private static readonly Cashier_DelegateTraining cashier = new();

    private event ShopDelegate Store;
    private delegate void ShopDelegate(ShoppingCart_DelegateTraining shoppingCart);

    private void Start()
    {
        Debug.Log(typeof(string).Assembly.ImageRuntimeVersion);

        Store += customer.ShopHygiene;
        Store += customer.ShopGroceries;
        Store += customer.ShopGuns;
        Store += customer.ShopClothes;
        Store += customer.ShopMeat;

        Store += (_) =>
                    {
                        shoppingCart.AddProduct(new Product_DelegateTraining("Kremidi", 1.42f));
                    };

        Store -= customer.ShopGuns;

        Store?.Invoke(shoppingCart);

        cashier.CheckOut(shoppingCart, cashier.CalculateTotal, cashier.GiveReceipt);
    }
}
