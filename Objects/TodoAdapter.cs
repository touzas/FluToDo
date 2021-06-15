using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FluToDo
{
    public class TodoAdapter : BaseAdapter<TodoItem>
    {
        private readonly List<TodoItem> m_items = new List<TodoItem>();
        Activity m_context;

        public TodoAdapter(Activity context, List<TodoItem> items)
            : base()
        {
            m_context = context;
            m_items = items;
        }

        public override TodoItem this[int position]
        {
            get
            {
                if (m_items == null && Count <= position)
                    return null;

                return m_items[position];
            }
        }

        public override int Count
        {
            get
            {
                if (m_items != null)
                    return m_items.Count();
                return 0;
            }
        }

        public void Add(TodoItem item)
        {
            m_items.Add(item);
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public bool RemoveItem(int position)
        {
            if (m_items == null && Count <= position)
                return false;

            var item = this[position];
            return m_items.Remove(item);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = m_items[position];
            View view = convertView;            
            if (view == null)
                view = m_context.LayoutInflater.Inflate(Resource.Layout.todoItem_row, null);

            view.Selected = item.IsComplete;
            view.FindViewById<TextView>(Resource.Id.tbRowTitle).Text = item.Name;
            var imgComlete = view.FindViewById<ImageView>(Resource.Id.imgComplete);
            imgComlete.Visibility = item.IsComplete ? ViewStates.Visible : ViewStates.Invisible;
            return view;
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }
    }
}