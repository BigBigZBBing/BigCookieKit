using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Algorithm
{
    public class BPlusTree<TK, TV>
        where TK : IComparable
    {
        private BPlusNode root;
        public int m_leaf;

        public BPlusTree(int leaf)
        {
            m_leaf = leaf;
            root = new BPlusNode(this, m_leaf);
        }

        internal void RootAs(BPlusNode root)
        {
            this.root = root;
        }

        public BPlusLeaf Insert(TK key, TV value)
        {
            return root.InsertLeaf(key, value);
        }

        public BPlusLeaf Search(TK key)
        {
            return root.Search(key);
        }

        public void Remove(TK key)
        {

        }

        public class BPlusNode
        {
            /// <summary>
            /// 叶子节点内最大叶子数
            /// </summary>
            internal int m_maxleaf;

            /// <summary>
            /// 对于节点树的关联
            /// </summary>
            internal BPlusTree<TK, TV> m_tree;

            /// <summary>
            /// 每个节点标识符
            /// </summary>
            public readonly Guid identifier = Guid.NewGuid();

            internal BPlusNode(BPlusTree<TK, TV> tree, int maxleaf)
            {
                m_tree = tree;
                m_maxleaf = maxleaf;
            }

            /// <summary>
            /// 链式首位
            /// </summary>
            internal BPlusLeaf First;

            /// <summary>
            /// 叶子的上级节点
            /// </summary>
            internal BPlusNode Parent;

            /// <summary>
            /// 关联分裂后指针未指向更大的节点
            /// </summary>
            internal BPlusNode Larger;

            /// <summary>
            /// 获取节点内叶子的数量
            /// </summary>
            internal int Count
            {
                get
                {
                    int res = 0;
                    var leaf = First;
                    while (leaf != null)
                    {
                        leaf = leaf.Next;
                        res++;
                    }
                    return res;
                }
            }

            /// <summary>
            /// 根据索引获取节点中的叶子
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            internal BPlusLeaf this[int index]
            {
                get
                {
                    var leaf = First;
                    while (leaf != null && index > 0)
                    {
                        leaf = leaf.Next;
                        index--;
                    }
                    return leaf;
                }
            }

            public BPlusLeaf InsertLeaf(TK key, TV value)
            {
                if (First == null) return First = new BPlusLeaf(key, value) { Belong = this };
                var leaf = FindTreeLeaf(key);

                // 索引是相同的情况下 把数据推入非叶子节点
                if (leaf.CompareTo(key) == 0) return leaf.Push(value);
                var n_leaf = new BPlusLeaf(key, value) { Belong = this };
                // 判断一下新叶子是前置还是后置
                if (leaf.CompareTo(key) == 1) BeforePlaceLeaf(leaf, n_leaf);
                else if (leaf.CompareTo(key) == -1) AfterPlaceLeaf(leaf, n_leaf);
                else throw new Exception("DecisionLeaf!");
                // 校验树的平衡性并修正
                leaf.Belong.EnsureBalance();

                return n_leaf;
            }

            public void RemoveLeaf(TK key)
            {
                var leaf = FindNodeLeaf(key);
                if (leaf == null) return;
                // 命中索引节点
                if (leaf.Key.Equals(key))
                {
                    // 脱离链式关系
                    var prev = leaf.Previous;
                    var next = leaf.Next;
                    prev.Next = next;
                    next.Previous = prev;

                    if (leaf.Belong.Count <= 2)
                    {

                    }
                }
                var pointer = leaf.Pointer?.FindTreeLeaf(key);
                pointer?.Belong.RemoveLeaf(key);
            }

            /// <summary>
            /// 移动指定叶子到节点中
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="includelarger"></param>
            /// <returns></returns>
            internal BPlusLeaf MoveLeafToNode(TK key, List<TV> value, bool includelarger = true)
            {
                if (First == null) return First = new BPlusLeaf(key, value) { Belong = this };
                var leaf = FindNodeLeaf(key, includelarger);

                // 索引是相同的情况下 把数据推入非叶子节点
                if (leaf.CompareTo(key) == 0) return leaf.Push(value);
                var n_leaf = new BPlusLeaf(key, value) { Belong = this };
                // 判断一下新叶子是前置还是后置
                if (leaf.CompareTo(key) == 1) BeforePlaceLeaf(leaf, n_leaf);
                else if (leaf.CompareTo(key) == -1) AfterPlaceLeaf(leaf, n_leaf);
                else throw new Exception("DecisionLeaf!");

                leaf.Belong.EnsureBalance();

                return n_leaf;
            }

            /// <summary>
            /// 查找匹配的叶子
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            internal BPlusLeaf Search(TK key)
            {
                var leaf = FindTreeLeaf(key);
                return leaf.Equals(key) ? leaf : null;
            }

            /// <summary>
            /// 查找整棵树内最合适的叶子
            /// </summary>
            /// <param name="key"></param>
            /// <param name="includelarger"></param>
            /// <returns></returns>
            internal BPlusLeaf FindTreeLeaf(TK key, bool includelarger = true)
            {
                var leaf = FindNodeLeaf(key, includelarger);
                if (leaf == null) return First;
                return leaf.Pointer?.FindTreeLeaf(key) ?? leaf;
            }

            /// <summary>
            /// 查找节点中最合适的叶子
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            internal BPlusLeaf FindNodeLeaf(TK key, bool includelarger = true)
            {
                var counter = First;
                while (counter != null)
                {
                    // 一模一样的叶子
                    if (counter.Key.CompareTo(key) == 0)
                    {
                        return counter;
                    }
                    // 叶子索引小于目标索引
                    else if (counter.CompareTo(key) == -1)
                    {
                        // 不存在下一级叶子就返回最后一个叶子
                        if (counter.Next == null
                            && (!includelarger || counter.Belong.Larger == null)) return counter;
                        counter = counter.Next ?? counter.Belong.Larger.First;
                    }
                    // 叶子索引大于目标索引
                    else if (counter.CompareTo(key) == 1)
                    {
                        return counter;
                    }
                    else throw new Exception("DecisionLeaf!");
                }
                return null;
            }

            /// <summary>
            /// 在叶子之前放置新叶子
            /// </summary>
            /// <param name="leaf"></param>
            /// <param name="n_leaf"></param>
            internal void BeforePlaceLeaf(BPlusLeaf leaf, BPlusLeaf n_leaf)
            {
                var previous = leaf.Previous;
                leaf.Previous = n_leaf;
                n_leaf.Previous = previous;
                n_leaf.Next = leaf;
                n_leaf.Belong = leaf.Belong;
                previous.Next = n_leaf;
            }

            /// <summary>
            /// 在叶子之后放置新叶子
            /// </summary>
            /// <param name="leaf"></param>
            /// <param name="n_leaf"></param>
            internal void AfterPlaceLeaf(BPlusLeaf leaf, BPlusLeaf n_leaf)
            {
                var next = leaf.Next;
                leaf.Next = n_leaf;
                n_leaf.Next = next;
                n_leaf.Previous = leaf;
                n_leaf.Belong = leaf.Belong;
            }

            /// <summary>
            /// 分裂节点中的叶子到指定节点
            /// </summary>
            /// <param name="index"></param>
            /// <param name="node"></param>
            internal void DissociateLeafToIndex(int index, BPlusNode node)
            {
                var leaf = First;
                // 循环更新之前到升级的叶子到新节点
                while (index >= 0 && leaf != null)
                {
                    var n_leaf = node.MoveLeafToNode(leaf.Key, leaf.Data, false);
                    n_leaf.Pointer = leaf.Pointer;
                    var next = leaf.Next;
                    // 清理指针
                    leaf.Clear();
                    leaf = next;
                    index--;
                }
                // 升级的叶子之后需要在原节点中去除联系
                leaf.Previous = null;
                leaf.Belong.First = leaf;

                // 升级的叶子之后的叶子
                // 如果没有父节点 被视为Larger节点
                node.Parent.Larger = leaf.Belong;
                leaf.Belong.Parent = node.Parent;
            }

            /// <summary>
            /// 多层挤出B+树的Root节点
            /// </summary>
            internal void PutNodeToRoot(BPlusNode node)
            {
                var m_node = node;
                while (m_node.Parent != null)
                    m_node = m_node.Parent;
                m_tree.RootAs(m_node);
            }

            /// <summary>
            /// 确保叶子节点平衡
            /// </summary>
            internal void EnsureBalance()
            {
                // 叶子超过最大叶子数进行升级
                if (Count > m_maxleaf)
                {
                    var upindex = Math.Round((double)m_maxleaf / 2);
                    var leaf = this[(int)upindex];

                    // 升级叶子 没有父节点就创建
                    var upleaf = (Parent ?? new BPlusNode(m_tree, m_maxleaf))
                        .MoveLeafToNode(leaf.Key, null, false);
                    upleaf.Pointer = new BPlusNode(m_tree, m_maxleaf) { Parent = upleaf.Belong };

                    // 升级叶子后需要断开叶子节点原有的的Larger联系
                    upleaf.Belong.Larger = null;

                    // 升级后多层挤出的方式获取最高级别节点作为Root
                    PutNodeToRoot(upleaf.Belong);

                    // 分裂升级叶子节点的下级叶子
                    DissociateLeafToIndex((int)upindex, upleaf.Pointer);
                }
            }
        }

        /// <summary>
        /// B+Tree叶子模型
        /// 设计为链式关系
        /// </summary>
#pragma warning disable CS0659 // “BPlusTree<TK, TV>.BPlusLeaf”重写 Object.Equals(object o) 但不重写 Object.GetHashCode()
        public class BPlusLeaf
#pragma warning restore CS0659 // “BPlusTree<TK, TV>.BPlusLeaf”重写 Object.Equals(object o) 但不重写 Object.GetHashCode()
        {
            internal BPlusLeaf() { }

            /// <summary>
            /// 上一个叶子
            /// </summary>
            internal BPlusLeaf Previous;

            /// <summary>
            /// 下一个叶子
            /// </summary>
            internal BPlusLeaf Next;

            /// <summary>
            /// 叶子隶属的节点
            /// </summary>
            internal BPlusNode Belong;

            /// <summary>
            /// 叶子节点的指向
            /// </summary>
            internal BPlusNode Pointer;

            /// <summary>
            /// 索引的键
            /// </summary>
            public TK Key;

            /// <summary>
            /// 标识符
            /// </summary>
            public readonly Guid identifier = Guid.NewGuid();

            /// <summary>
            /// 数据块
            /// </summary>
            public List<TV> Data = new List<TV>();

            internal BPlusLeaf(TK key, TV value)
            {
                this.Key = key;
                this.Push(value);
            }

            internal BPlusLeaf(TK key, List<TV> value)
            {
                this.Key = key;
                if (value != null)
                    this.Push(value);
            }

            /// <summary>
            /// 往数据块中推送
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            internal BPlusLeaf Push(TV value)
            {
                if (value.Equals(default(TV))) return null;
                Data.Add(value);
                return this;
            }

            /// <summary>
            /// 往数据块中推送
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            internal BPlusLeaf Push(List<TV> value)
            {
                Data.AddRange(value);
                return this;
            }

            /// <summary>
            /// 清理内存防止指针导致内存泄漏
            /// </summary>
            internal void Clear()
            {
                Key = default(TK);
                Data = null;
                Previous = null;
                Next = null;
                Belong = null;
                Pointer = null;
                GC.Collect();
            }

            public int CompareTo(object obj)
            {
                return Key.CompareTo(obj);
            }

            public override bool Equals(object obj)
            {
                return Key.Equals(obj);
            }
        }
    }
}
