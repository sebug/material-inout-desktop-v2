namespace material_inout_desktop_v2.ViewModels;

public class ReturnMaterialViewModel : ViewModelBase, IQueryAttributable
{
    public int VoucherID
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(VoucherID));
            }
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        string? voucherIDString = query["VoucherID"] as string;
        if (!String.IsNullOrEmpty(voucherIDString) && int.TryParse(voucherIDString, out int vid))
        {
            VoucherID = vid;
        }
    }
}
