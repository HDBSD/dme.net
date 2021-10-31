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
        private MemWatchEntry m_parent;

        #endregion

        #region Public

        public MemWatchTreeNode(MemWatchEntry entry, MemWatchTreeNode parent,
                         bool isGroup = false, string groupName = "")
        {

        }

        public MemWatchTreeNode(ref MemWatchTreeNode node)
        {

        }

        public MemWatchTreeNode()
        {

        }

        public bool isGroup()
        {
            return false;
        }

        public string getGroupName()
        {
            return m_groupName;
        }

        public void setGroupName(ref string groupName)
        {

        }

        public MemWatchEntry getEntry()
        {
            return (MemWatchEntry)m_entry;
        }

        public void setEntry(MemWatchEntry entry)
        {

        }

        public List<MemWatchTreeNode> getChildren()
        {
            return this.m_children;
        }

        public void setChildren(List<MemWatchTreeNode> children)
        {

        }

        public MemWatchTreeNode getParent()
        {
            return this;
        }

        public int getRow()
        {
            return 0;
        }

        public bool isValueEditing()
        {
            return false;
        }

        public bool hasChildren()
        {
            return false;
        }

        public int childrenCount()
        {
            return 0;
        }

        public void setValueEditing(bool valueEditing)
        {

        }

        public void appendChild(MemWatchTreeNode node)
        {

        }

        public void insertChild(int row, MemWatchTreeNode node)
        {

        }

        public void removeChild(int row)
        {

        }

        public void clearAllChild()
        {

        }
        
        //public void readFromJson(QJsonObject& json, MemWatchTreeNode parent = null);
        //public void writeToJson(QJsonObject& json) const;
        //public QString writeAsCSV() const;

        #endregion
    }
}
