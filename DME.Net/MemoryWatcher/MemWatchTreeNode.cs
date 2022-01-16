using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinComm.MemoryWatcher
{
    public class MemWatchTreeNode
    {
        #region Private

        private bool m_isGroup;
        private bool m_isValueEditing = false;
        private string m_groupName;
        private MemWatchEntry m_entry;
        private List<MemWatchTreeNode> m_children;
        private MemWatchTreeNode m_parent;

        #endregion

        #region Public

        public MemWatchTreeNode(MemWatchEntry entry, MemWatchTreeNode parent,
                         bool isGroup = false, string groupName = "")
        {

        }

        public MemWatchTreeNode(ref MemWatchTreeNode node)
        {
            m_isGroup = node.m_isGroup;
            m_isValueEditing = node.m_isValueEditing;
            m_groupName = node.m_groupName;
            m_entry = node.m_entry;
            m_children = node.m_children;
            m_parent = node.m_parent;
        }

        ~MemWatchTreeNode()
        {
            if (hasChildren())
                m_children.Clear();

            m_parent = null;
            m_entry = null;
        }

        public bool isGroup()
        {
            return false;
        }

        public bool hasChildren()
        {
            return (m_children.Count >= 1);
        }

        public int childrenCount()
        {
            return m_children.Count;
        }

        public bool isValueEditing()
        {
            return m_isValueEditing;
        }

        public void setValueEditing(bool valueEditing)
        {
            m_isValueEditing = valueEditing;
        }

        public string getGroupName()
        {
            return m_groupName;
        }

        public void setGroupName(ref string groupName)
        {
            m_groupName = groupName;
        }

        public MemWatchEntry getEntry()
        {
            return m_entry;
        }

        public void setEntry(MemWatchEntry entry)
        {
            m_entry = null;
            m_entry = entry;
        }

        public List<MemWatchTreeNode> getChildren()
        {
            return m_children;
        }

        public void setChildren(List<MemWatchTreeNode> children)
        {
            m_children = children;
        }

        public MemWatchTreeNode getParent()
        {
            return m_parent;
        }

        public int getRow()
        {
            if (m_parent == null)
                return m_parent.m_children.IndexOf(this);

            return 0;
        }

        public void appendChild(MemWatchTreeNode node)
        {
            m_children.Append(node);
            node.m_parent = this;
        }

        public void insertChild(int row, MemWatchTreeNode node)
        {
            m_children.Insert(row, node);
            node.m_parent = this;
        }

        public void removeChild(int row)
        {
            m_children.RemoveAt(row);
        }

        public void clearAllChild()
        {
            m_children.Clear();
        }

        public void readFromJson(string json, MemWatchTreeNode parent = null)
        {
            throw new NotImplementedException("json importing not implemented");
        }
        public void writeToJson(string json)
        {
            throw new NotImplementedException("json exporting not implemented");
        }

        public string writeAsCSV()
        {
            throw new NotImplementedException("CSV exporting not implemented");
            return "";
        }

        #endregion
    }
}
