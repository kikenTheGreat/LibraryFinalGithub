using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Final
{
    public static class GlobalEvents
    {




        // 🔔 Overdue
        public static event Action? OverdueDataChanged;
        public static void RaiseOverdueDataChanged() => OverdueDataChanged?.Invoke();

        // 📚 BooksAcq (total books)
        public static event Action? BooksDataChanged;
        public static void RaiseBooksDataChanged() => BooksDataChanged?.Invoke();

        // 📖 IssueBooks (borrowed)
        public static event Action? BorrowedDataChanged;
        public static void RaiseBorrowedDataChanged() => BorrowedDataChanged?.Invoke();

        // 🗃️ BooksArchive (archived)
        public static event Action? ArchivedDataChanged;
        public static void RaiseArchivedDataChanged() => ArchivedDataChanged?.Invoke();


        public static event Action? PenaltiesDataChanged;
        public static void RaisePenaltiesDataChanged() => PenaltiesDataChanged?.Invoke();




     

    }
}
