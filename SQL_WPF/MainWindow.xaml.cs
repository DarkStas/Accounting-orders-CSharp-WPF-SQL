using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.ComponentModel;
using MySql.Data.MySqlClient;


namespace SQL_WPF
{

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string mDbPath = "server=localhost;user=root;database=database2;password=0000";
		MySqlConnection mConn;
		MySqlDataAdapter mAdapter;
		DataTable mTable;
		MySqlCommand mCmd;
		int tabindex = -1;
		int currenttabindex = -1;
		string editcategoryid, editcategorydate, editcategoryclient, editcategorysum/*, *//*editcategoryorderlist*/; /*id, date, client, sum, order_list*/
		string edititemorderid, edititemid, edititemname, edititemamount, edititempricepiece, edititemcost; /*order_id, item_name, amount, price_piece, cost*/
		string NowOrderId;
		double CalcSumTemp, CalcCost;
		//MySqlDataReader reader;
		object item;
		int MaxId = 0;
		public MainWindow()
		{
			InitializeComponent();
			FillDataGrid();

		}

		private void FillDataGrid()
		{
			//mDbPath = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.ConnectionStrings["mDbPath"].ConnectionString;
			try
			{
				using (mConn = new MySqlConnection(mDbPath))
				{
					mConn.Open();


					if (tabindex == 0 && currenttabindex != 0)//Category Table Tab in Database Tab
					{
						mCmd = new MySqlCommand("SELECT id, date, client, sum FROM order1 order by id", mConn);

						mCmd.ExecuteNonQuery();
						mAdapter = new MySqlDataAdapter(mCmd);
						mTable = new DataTable("order1");
						mAdapter.Fill(mTable);
						mConn.Close();
						categoryDataGridatDatabase.ItemsSource = mTable.DefaultView;
						mAdapter.Update(mTable);

						this.categoryDataGridatDatabase.Columns[0].Header = "Id";
						this.categoryDataGridatDatabase.Columns[1].Header = "Date";
						this.categoryDataGridatDatabase.Columns[2].Header = "Client";
						this.categoryDataGridatDatabase.Columns[3].Header = "Sum";
						
					}
					else if (tabindex == 1 && currenttabindex != 1)//Items Table Tab in Database Tab
					{
						
						//this.RefreshSumOnOrderTab.ClearValue(Button.BackgroundProperty);
						//this.RefreshCost.ClearValue(Button.BackgroundProperty);

						NowOrderId = editcategoryid;
						if (NowOrderId == null) return;
						//string azaz = OrderListColumn.DataContext as UserControl;
						mCmd = new MySqlCommand("SELECT order_id, id, item_name, amount, price_piece, cost FROM orders_list WHERE order_id = " + NowOrderId, mConn);

						mCmd.ExecuteNonQuery();
						mAdapter = new MySqlDataAdapter(mCmd);
						mTable = new DataTable("orders_list");
						mAdapter.Fill(mTable);
						mConn.Close();
						itemsDataGridatDatabase.ItemsSource = mTable.DefaultView;
						mAdapter.Update(mTable);

						this.itemsDataGridatDatabase.Columns[0].Header = "Order Id";
						this.itemsDataGridatDatabase.Columns[1].Header = "Id";
						this.itemsDataGridatDatabase.Columns[2].Header = "Item Name";
						this.itemsDataGridatDatabase.Columns[3].Header = "Amount";
						this.itemsDataGridatDatabase.Columns[4].Header = "Price/Piece";
						this.itemsDataGridatDatabase.Columns[5].Header = "Cost";
						CalcAndShowSum();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error Message : " + ex);
			}

		}
		#region CategoryTable Functions

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Button_RefreshCost()
		{


			try
			{
				using (mConn = new MySqlConnection(mDbPath))
				{

					mConn.Open();
					//Insert Command

					CalcCost = 1;
					//mCmd = new MySqlCommand("update orders_list SET cost ='" + CalcCost.ToString() + "' WHERE order_id ='" + NowOrderId + "' AND id = " + (i), mConn);
					//mCmd.ExecuteNonQuery();
					//mCmd = null;
					////Select Command
					mCmd = new MySqlCommand("SELECT order_id, id, item_name, amount, price_piece, cost FROM orders_list WHERE order_id = " + NowOrderId, mConn);
					mCmd.ExecuteNonQuery();
					mAdapter = new MySqlDataAdapter(mCmd);
					mTable = new DataTable("orders_list");



					mAdapter.Fill(mTable);
					mConn.Close();
					itemsDataGridatDatabase.ItemsSource = mTable.DefaultView;
					mAdapter.Update(mTable);
					this.itemsDataGridatDatabase.Columns[0].Header = "Order Id";
					this.itemsDataGridatDatabase.Columns[1].Header = "Id";
					this.itemsDataGridatDatabase.Columns[2].Header = "Item Name";
					this.itemsDataGridatDatabase.Columns[3].Header = "Amount";
					this.itemsDataGridatDatabase.Columns[4].Header = "Price/Piece";
					this.itemsDataGridatDatabase.Columns[5].Header = "Cost";

					List<int> IdsList = new List<int>();
					mConn.Open();

					mCmd = new MySqlCommand("SELECT id FROM orders_list WHERE order_id ='" + NowOrderId + "' AND id > 0", mConn);
					MySqlDataReader reader = mCmd.ExecuteReader();
					while (reader.Read())
					{
						IdsList.Add(Convert.ToInt32(reader[0]));
					}
					reader.Close();


					for (int i = 0; i <= mTable.Rows.Count - 1; i++)
					{
						CalcCost = 0;
						CalcCost = Convert.ToDouble(mTable.Rows[i][3]) * Convert.ToDouble(mTable.Rows[i][4]);
						mTable.Rows[i][5] = CalcCost;
						mCmd = new MySqlCommand("update orders_list SET cost ='" + CalcCost.ToString() + "' WHERE order_id ='" + NowOrderId + "' AND id = " + IdsList[i].ToString(), mConn);
						mCmd.ExecuteNonQuery();
						mCmd = null;


					}

					mConn.Close();
					IdsList.Clear();
					CalcAndShowSum();
					Button_RefreshSumOnOrderTab();
				}
				//RefreshCost.Background = Brushes.Green;
			}
			catch (Exception ex)
			{
				//RefreshCost.Background = Brushes.Red;
				MessageBox.Show("Error Message : " + ex);
			}
		}

		private void CalcAndShowSum()
		{
			for (int i = 0; i <= mTable.Rows.Count - 1; i++)
			{
				CalcSumTemp += Convert.ToDouble(mTable.Rows[i][5]);
			}
			//CalcSum.Text = mTable.Rows[1][5].ToString();
			CalcSum.Text = CalcSumTemp.ToString();
			CalcSumTemp = 0;
		}

		private void Button_RefreshSumOnOrderTab()
		{
			try
			{
				using (mConn = new MySqlConnection(mDbPath))
				{
					mConn.Open();
					//Insert Command
					mCmd = new MySqlCommand("update order1 SET sum ='" + this.CalcSum.Text.ToString() + "' where id =" + NowOrderId, mConn);
					mCmd.ExecuteNonQuery();
					mCmd = null;
					//Select Command
					mCmd = new MySqlCommand("SELECT id, date, client, sum FROM order1 order by id", mConn);
					mCmd.ExecuteNonQuery();
					mAdapter = new MySqlDataAdapter(mCmd);
					mTable = new DataTable("order1");

					mAdapter.Fill(mTable);
					mConn.Close();
					categoryDataGridatDatabase.ItemsSource = mTable.DefaultView;
					//mAdapter.Update(mTable);
					this.categoryDataGridatDatabase.Columns[0].Header = "Id";
					this.categoryDataGridatDatabase.Columns[1].Header = "Date";
					this.categoryDataGridatDatabase.Columns[2].Header = "Client";
					this.categoryDataGridatDatabase.Columns[3].Header = "Sum";
					

					//mConn.Close();
				}
				//RefreshSumOnOrderTab.Background = Brushes.Green;
			}
			catch (Exception ex)
			{
				//RefreshSumOnOrderTab.Background = Brushes.Red;
				MessageBox.Show("Error Message : " + ex);
			}
		}

		/*WHEN PRESS Open OrderList*/
		private void Button_OpenOrderList_CategoryElement_Click(object sender, RoutedEventArgs e)
		{
			if (editcategoryid == null) return;
			itemtableTab.Visibility = System.Windows.Visibility.Visible;
			

			maindatabaseTabCtrl.SelectedIndex = 1;
			tabindex = 1;
			currenttabindex = 1;
			categorytableTab.Visibility = System.Windows.Visibility.Hidden;

			FillDataGrid();
			

		}

		/*WHEN PRESS ADD*/
		private void Button_Add_CategoryElement_Click(object sender, RoutedEventArgs e)
		{
			AddCategoryElementInputBox.Visibility = System.Windows.Visibility.Visible;
		}

		/*WHEN PRESS ADD -> Submit*/
		private void Button_Submit_Category(object sender, RoutedEventArgs e)
		{
			AddCategoryElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
			//Do Something Here
			//mDbPath = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.ConnectionStrings["mDbPath"].ConnectionString;
			try
			{
				using (mConn = new MySqlConnection(mDbPath))
				{
					mConn.Open();
					mCmd = new MySqlCommand("SELECT MAX(id) FROM order1", mConn);
					MaxId = Convert.ToInt32(mCmd.ExecuteScalar());
					
					//Insert Command
					mCmd = new MySqlCommand("insert into order1 (id, date, client) values('" + (MaxId+1) + "','" + Convert.ToDateTime(this.InputCategoryDateBox.Text).ToString("yyyy/MM/dd") + "','" + this.InputCategoryClientBox.Text.ToString() /*+ "','" + this.InputCategorySumBox.Text.ToString()*/ /*+ "','" + this.InputCategoryOrderlistBox.Text.ToString()*/ + "')", mConn);
					MaxId = 0;
					mCmd.ExecuteNonQuery();
					mCmd = null;
					//Select Command
					mCmd = new MySqlCommand("SELECT id, date, client, sum FROM order1 order by id", mConn);
					mCmd.ExecuteNonQuery();
					mAdapter = new MySqlDataAdapter(mCmd);
					mTable = new DataTable("order1");

					mAdapter.Fill(mTable);
					mConn.Close();
					categoryDataGridatDatabase.ItemsSource = mTable.DefaultView;
					mAdapter.Update(mTable);
					this.categoryDataGridatDatabase.Columns[0].Header = "Id";
					this.categoryDataGridatDatabase.Columns[1].Header = "Date";
					this.categoryDataGridatDatabase.Columns[2].Header = "Client";
					this.categoryDataGridatDatabase.Columns[3].Header = "Sum";
					

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error Message : " + ex);
			}

			//ENd Here
			InputCategoryIdBox.Text = String.Empty;
			InputCategoryDateBox.Text = String.Empty;
			InputCategoryClientBox.Text = String.Empty;
			//InputCategorySumBox.Text = String.Empty;
			//InputCategoryOrderlistBox.Text = String.Empty;

		}

		/*WHEN PRESS ADD -> Cancel*/
		private void Button_Cancel_Category(object sender, RoutedEventArgs e)
		{
			AddCategoryElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
			InputCategoryIdBox.Text = String.Empty;
			InputCategoryDateBox.Text = String.Empty;
			InputCategoryClientBox.Text = String.Empty;
			//InputCategorySumBox.Text = String.Empty;
			//InputCategoryOrderlistBox.Text = String.Empty;
		}

		/*WHEN PRESS EDIT*/
		private void Button_Edit_CategoryElement_Click(object sender, RoutedEventArgs e)
		{
			InputEditCategoryIdBox.Text = editcategoryid;
			InputEditDateBox.Text = editcategorydate;
			InputEditClientBox.Text = editcategoryclient;
			//InputEditSumBox.Text = editcategorysum;
			//InputEditOrderlistBox.Text = editcategoryorderlist;
			EditCategoryElementInputBox.Visibility = System.Windows.Visibility.Visible;

		}

		/*WHEN PRESS EDIT -> Submit*/
		private void ButtonEdit_Update_Category(object sender, RoutedEventArgs e)
		{

			//Do Something 
			if (!InputEditCategoryIdBox.Text.Equals(editcategoryid) || !InputEditDateBox.Text.Equals(editcategorydate) || !InputEditClientBox.Text.Equals(editcategoryclient) /*|| !InputEditSumBox.Text.Equals(editcategorysum)*//* || !InputEditOrderlistBox.Text.Equals(editcategoryorderlist)*/)
			{
				EditCategoryElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
				try
				{
					using (mConn = new MySqlConnection(mDbPath))
					{
						mConn.Open();
						//Insert Command
						mCmd = new MySqlCommand("update order1 SET date ='" + Convert.ToDateTime(this.InputEditDateBox.Text).ToString("yyyy/MM/dd") + "', client ='" + this.InputEditClientBox.Text.ToString() + /*"', sum ='" + this.InputEditSumBox.Text.ToString() +*/ /*"', order_list ='" + this.InputEditOrderlistBox.Text.ToString() +*/ "' where id =" + Int32.Parse(this.InputEditCategoryIdBox.Text.ToString()), mConn);
						mCmd.ExecuteNonQuery();
						mCmd = null;
						//Select Command
						mCmd = new MySqlCommand("SELECT id, date, client, sum FROM order1 order by id", mConn);
						mCmd.ExecuteNonQuery();
						mAdapter = new MySqlDataAdapter(mCmd);
						mTable = new DataTable("order1");

						mAdapter.Fill(mTable);
						mConn.Close();
						categoryDataGridatDatabase.ItemsSource = mTable.DefaultView;
						//mAdapter.Update(mTable);
						this.categoryDataGridatDatabase.Columns[0].Header = "Id";
						this.categoryDataGridatDatabase.Columns[1].Header = "Date";
						this.categoryDataGridatDatabase.Columns[2].Header = "Client";
						this.categoryDataGridatDatabase.Columns[3].Header = "Sum";
						

						//mConn.Close();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error Message : " + ex);
				}
				InputEditCategoryIdBox.Text = String.Empty;
				InputEditDateBox.Text = String.Empty;
				InputEditClientBox.Text = String.Empty;
				//InputEditSumBox.Text = String.Empty;
				//InputEditOrderlistBox.Text = String.Empty;
				editcategoryid = String.Empty;
				editcategorydate = String.Empty;
				editcategoryclient = String.Empty;
				editcategorysum = String.Empty;
				//editcategoryorderlist = String.Empty;

			}
			else
			{
				MessageBox.Show("No Entry Changed.");
			}
		}

		/*WHEN PRESS EDIT -> Cancel*/
		private void ButtonEdit_Cancel_Category(object sender, RoutedEventArgs e)
		{
			EditCategoryElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
			InputEditCategoryIdBox.Text = String.Empty;
			InputEditDateBox.Text = String.Empty;
			InputEditClientBox.Text = String.Empty;
			//InputEditSumBox.Text = String.Empty;
			//InputEditOrderlistBox.Text = String.Empty;
		}

		/*WHEN PRESS DELETE*/
		private void Button_Delete_CategoryElement_Click(object sender, RoutedEventArgs e)
		{
			DeleteCategoryElementInputBox.Visibility = System.Windows.Visibility.Visible;
		}

		/*WHEN PRESS DELETE -> Yes*/
		private void Button_Delete_Yes_Category(object sender, RoutedEventArgs e)
		{
			//Save Changes
			//mDbPath = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.ConnectionStrings["mDbPath"].ConnectionString;
			try
			{
				using (mConn = new MySqlConnection(mDbPath))
				{
					mConn.Open();
					//Insert Command
					mCmd = new MySqlCommand("delete from order1 where id = " + Int32.Parse(editcategoryid), mConn);
					mCmd.ExecuteNonQuery();
					mCmd = null;
					//Select Command
					mCmd = new MySqlCommand("SELECT id, date, client, sum FROM order1 order by id", mConn);
					mCmd.ExecuteNonQuery();
					mAdapter = new MySqlDataAdapter(mCmd);
					mTable = new DataTable("order1");

					mAdapter.Fill(mTable);
					mConn.Close();
					categoryDataGridatDatabase.ItemsSource = mTable.DefaultView;
					mAdapter.Update(mTable);
					this.categoryDataGridatDatabase.Columns[0].Header = "Id";
					this.categoryDataGridatDatabase.Columns[1].Header = "Date";
					this.categoryDataGridatDatabase.Columns[2].Header = "Client";
					this.categoryDataGridatDatabase.Columns[3].Header = "Sum";
					
					DeleteCategoryElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
					
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error Message : " + ex);
			}
		}

		/*WHEN PRESS DELETE -> NO*/
		private void Button_Delete_No_Category(object sender, RoutedEventArgs e)
		{
			DeleteCategoryElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
		}
		#endregion



		#region ItemsTable Functions

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/*WHEN PRESS Close OrderList*/
		private void Button_CloseOrderList_CategoryElement_Click(object sender, RoutedEventArgs e)
		{
			NowOrderId = String.Empty;
			//itemtableTab.IsSelected = false;
			itemtableTab.Visibility = System.Windows.Visibility.Collapsed;
			categorytableTab.Visibility = System.Windows.Visibility.Visible;

			//databaseTab.IsSelected = true;
			maindatabaseTabCtrl.SelectedIndex = 0;
			tabindex = 0;
			currenttabindex = 0;


			//this.RefreshSumOnOrderTab.ClearValue(Button.BackgroundProperty);
			//this.RefreshCost.ClearValue(Button.BackgroundProperty);
			FillDataGrid();

		}

		/*WHEN PRESS ADD*/
		private void Button_Add_ItemElement_Click(object sender, RoutedEventArgs e)
		{
			AddItemElementInputBox.Visibility = System.Windows.Visibility.Visible;
		}

		/*WHEN PRESS ADD -> Submit*/
		private void Button_Submit_Item(object sender, RoutedEventArgs e)
		{
			AddItemElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
			//Do Something Here
			//mDbPath = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.ConnectionStrings["mDbPath"].ConnectionString;
			try
			{
				using (mConn = new MySqlConnection(mDbPath))
				{
					mConn.Open();
					mCmd = new MySqlCommand("SELECT MAX(id) FROM orders_list where order_id = " + NowOrderId, mConn);
					try
					{
						MaxId = Convert.ToInt32(mCmd.ExecuteScalar());
					}
					catch
					{
						MaxId = 0;
					}
					
					//Insert Command
					mCmd = new MySqlCommand("insert into orders_list (order_id, id, item_name, amount, price_piece) values('" + Int32.Parse(NowOrderId) + "','" + (MaxId+1) + "','" + this.InputItemNameBox.Text.ToString() + "','" + Int32.Parse(this.InputAmountBox.Text.ToString()) + "','" + Int32.Parse(this.InputPricePieceBox.Text.ToString()) /*+ "','" + Int32.Parse(this.InputCostBox.Text.ToString())*/ + "')", mConn);
					MaxId = 0;
					mCmd.ExecuteNonQuery();
					mCmd = null;
					//Select Command
					mConn.Close();
					FillDataGrid();
					Button_RefreshCost();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error Message : " + ex);
			}

			//ENd Here
			InputItemIdBox.Text = String.Empty;
			InputItemNameBox.Text = String.Empty;
			InputAmountBox.Text = String.Empty;
			InputPricePieceBox.Text = String.Empty;
			//InputCostBox.Text = String.Empty;
			//this.RefreshSumOnOrderTab.ClearValue(Button.BackgroundProperty);
			//this.RefreshCost.ClearValue(Button.BackgroundProperty);

		}

		/*WHEN PRESS ADD -> Cancel*/
		private void Button_Cancel_Item(object sender, RoutedEventArgs e)
		{
			AddItemElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
			InputItemIdBox.Text = String.Empty;
			InputItemNameBox.Text = String.Empty;
			InputAmountBox.Text = String.Empty;
			InputPricePieceBox.Text = String.Empty;
			//InputCostBox.Text = String.Empty;
		}




		/*WHEN PRESS EDIT*/
		private void Button_Edit_ItemElement_Click(object sender, RoutedEventArgs e)
		{
			InputEditItemIdBox.Text = edititemid;
			InputEditItemNameBox.Text = edititemname;
			InputEditAmountBox.Text = edititemamount;
			InputEditPricePieceBox.Text = edititempricepiece;
			//InputEditCostBox.Text = edititemcost;
			EditItemElementInputBox.Visibility = System.Windows.Visibility.Visible;

		}

		/*WHEN PRESS Edit -> Submit*/
		private void ButtonEdit_Update_Item(object sender, RoutedEventArgs e)
		{

			//Do Something 
			if (!InputEditItemIdBox.Text.Equals(edititemid) || !InputEditItemNameBox.Text.Equals(edititemname) || !InputEditAmountBox.Text.Equals(edititemamount) || !InputEditPricePieceBox.Text.Equals(edititempricepiece) /*|| !InputEditCostBox.Text.Equals(edititemcost)*/)
			{
				EditItemElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
				try
				{
					using (mConn = new MySqlConnection(mDbPath))
					{
						mConn.Open();
						//Insert Command
						mCmd = new MySqlCommand("update orders_list SET item_name ='" + this.InputEditItemNameBox.Text.ToString() + "', amount ='" + Int32.Parse(this.InputEditAmountBox.Text.ToString()) + "', price_piece ='" + Int32.Parse(this.InputEditPricePieceBox.Text.ToString()) /*+ "', cost ='" + Int32.Parse(this.InputEditCostBox.Text.ToString())*/ + "' where order_id ='" + NowOrderId + "' AND id =" + this.InputEditItemIdBox.Text.ToString(), mConn);
						mCmd.ExecuteNonQuery();
						mCmd = null;
						//Select Command
						mConn.Close();
						FillDataGrid();
						Button_RefreshCost();
						//mConn.Close();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error Message : " + ex);
				}
				InputEditItemIdBox.Text = String.Empty;
				InputEditItemNameBox.Text = String.Empty;
				InputEditAmountBox.Text = String.Empty;
				InputEditPricePieceBox.Text = String.Empty;
				//InputEditCostBox.Text = String.Empty;
				edititemorderid = String.Empty;
				edititemname = String.Empty;
				edititemamount = String.Empty;
				edititempricepiece = String.Empty;
				edititemcost = String.Empty;
				//this.RefreshSumOnOrderTab.ClearValue(Button.BackgroundProperty);
				//this.RefreshCost.ClearValue(Button.BackgroundProperty);

			}
			else
			{
				MessageBox.Show("No Entry Changed.");
			}


		}

		/*WHEN PRESS Edit -> Cancel*/
		private void ButtonEdit_Cancel_Item(object sender, RoutedEventArgs e)
		{
			EditItemElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
			InputEditItemIdBox.Text = String.Empty;
			InputEditItemNameBox.Text = String.Empty;
			InputEditAmountBox.Text = String.Empty;
			InputEditPricePieceBox.Text = String.Empty;
		}


		/*WHEN PRESS DELETE*/
		private void Button_Delete_ItemElement_Click(object sender, RoutedEventArgs e)
		{
			DeleteItemElementInputBox.Visibility = System.Windows.Visibility.Visible;
		}

		/*WHEN PRESS DELETE -> YES*/
		private void Button_Yes_Item(object sender, RoutedEventArgs e)
		{
			//Save Changes
			//mDbPath = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.ConnectionStrings["mDbPath"].ConnectionString;
			try
			{
				using (mConn = new MySqlConnection(mDbPath))
				{
					mConn.Open();
					//Insert Command
					mCmd = new MySqlCommand("delete from orders_list where order_id = '" + NowOrderId + "' AND id =" + this.InputEditItemIdBox.Text.ToString(), mConn);
					mCmd.ExecuteNonQuery();
					mCmd = null;
					//Select Command
					mConn.Close();
					FillDataGrid();
					DeleteItemElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
					Button_RefreshCost();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error Message : " + ex);
			}
		}

		/*WHEN PRESS DELETE -> NO*/
		private void Button_No_Item(object sender, RoutedEventArgs e)
		{
			DeleteItemElementInputBox.Visibility = System.Windows.Visibility.Collapsed;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		#endregion



		private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
		{

			TabControl tabControl = sender as TabControl; // e.Source could have been used instead of sender as well
			TabItem item = tabControl.SelectedValue as TabItem;
			if (item.Name == "categorytableTab")
			{
				tabindex = 0;
				FillDataGrid();
				currenttabindex = 0;
			}
			else if (item.Name == "itemtableTab")
			{
				tabindex = 1;
				FillDataGrid();
				currenttabindex = 1;
			}


		}

		public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
		{
			var itemsSource = grid.ItemsSource as IEnumerable;
			if (null == itemsSource) yield return null;
			foreach (var item in itemsSource)
			{
				var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
				if (null != row) yield return row;
			}
		}

		private void categoryDataGridatDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (this.categoryDataGridatDatabase.SelectedItem != null)
				{
					item = this.categoryDataGridatDatabase.SelectedItem;
					editcategoryid = (this.categoryDataGridatDatabase.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
					this.InputEditCategoryIdBox.Text = editcategoryid;
					editcategorydate = (this.categoryDataGridatDatabase.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
					this.InputEditDateBox.Text = editcategorydate;
					editcategoryclient = (this.categoryDataGridatDatabase.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
					this.InputEditClientBox.Text = editcategoryclient;
					editcategorysum = (this.categoryDataGridatDatabase.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text;
					//this.InputEditSumBox.Text = editcategorysum;
					//editcategoryorderlist = (this.categoryDataGridatDatabase.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text;
					//this.InputEditOrderlistBox.Text = editcategoryorderlist;
				}
			}
			catch (Exception exp)
			{
				MessageBox.Show("Message:" + exp);
			}
		}


		/*This method is altered. Debugging may be required.*/
		private void itemsDataGridatDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (this.itemsDataGridatDatabase.SelectedItem != null)
				{
					item = this.itemsDataGridatDatabase.SelectedItem;
					edititemorderid = (this.itemsDataGridatDatabase.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
					this.InputEditItemIdBox.Text = edititemorderid;

					edititemid = (this.itemsDataGridatDatabase.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
					this.InputEditItemIdBox.Text = edititemid;

					edititemname = (this.itemsDataGridatDatabase.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
					this.InputEditItemNameBox.Text = edititemname;
					edititemamount = (this.itemsDataGridatDatabase.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text;
					this.InputEditAmountBox.Text = edititemamount;
					edititempricepiece = (this.itemsDataGridatDatabase.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text;
					this.InputEditPricePieceBox.Text = edititempricepiece;
					edititemcost = (this.itemsDataGridatDatabase.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text;
					//this.InputEditCostBox.Text = edititemcost;
				}
			}
			catch (Exception exp)
			{
				MessageBox.Show("Message:" + exp);
			}
		}


		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}

		

		private void Button_DataSearch_1(object sender, RoutedEventArgs e)
		{
			DateTime? selectedDate1 = DPDateSearchFrom.SelectedDate;
			DateTime? selectedDate2 = DPDateSearchTo.SelectedDate;

			try
			{
				using (mConn = new MySqlConnection(mDbPath))
				{
					mConn.Open();
					//Select Command
					if (selectedDate1.HasValue && selectedDate2.HasValue)
					{
						string Date1 = selectedDate1.Value.ToString("yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
						string Date2 = selectedDate2.Value.ToString("yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
						mCmd = new MySqlCommand("SELECT * FROM order1 WHERE date between '" + Date1 + "' AND '" + Date2 + "' order by id", mConn);
					}
					else if (selectedDate1.HasValue)
					{
						string Date1 = selectedDate1.Value.ToString("yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
						string Date2 = DateTime.Today.ToString("yyyy.MM.dd");
						mCmd = new MySqlCommand("SELECT * FROM order1 WHERE date between '" + Date1 + "' AND '" + Date2 + "' order by id", mConn);
					}
					else if (selectedDate2.HasValue)
					{
						string Date1 = "0000.01.01";
						string Date2 = selectedDate2.Value.ToString("yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
						mCmd = new MySqlCommand("SELECT * FROM order1 WHERE date between '" + Date1 + "' AND '" + Date2 + "' order by id", mConn);
					}
					else
					{
						mCmd = new MySqlCommand("SELECT id, date, client, sum, order_list FROM order1 order by id", mConn);

					}

					mCmd.ExecuteNonQuery();
					mAdapter = new MySqlDataAdapter(mCmd);
					mTable = new DataTable("order1");

					mAdapter.Fill(mTable);
					mConn.Close();
					categoryDataGridatDatabase.ItemsSource = mTable.DefaultView;
					//mAdapter.Update(mTable);
					this.categoryDataGridatDatabase.Columns[0].Header = "Id";
					this.categoryDataGridatDatabase.Columns[1].Header = "Date";
					this.categoryDataGridatDatabase.Columns[2].Header = "Client";
					this.categoryDataGridatDatabase.Columns[3].Header = "Sum";
					this.categoryDataGridatDatabase.Columns[4].Header = "Order list";

					Button_DataSearch.Background = Brushes.Green;
					//mConn.Close();
				}
				}
			catch (Exception ex)
			{
				MessageBox.Show("Error Message : " + ex);
			}

			
		}

		

		private void DPDateSearchTo_MouseLeave_1(object sender, SelectionChangedEventArgs e)
		{

			this.Button_DataSearch.ClearValue(Button.BackgroundProperty);
		}

	}
}
