using System.Collections.ObjectModel;

namespace JSONConverter;

public class TreeNode<T>
{
    public T Data { get; set; }
    public TreeNode<T> Parent { get; set; }
    public List<TreeNode<T>> Children { get; private set; }

    public TreeNode(T data)
    {
        Data = data;
        Children = new List<TreeNode<T>>();
    }

    // Adds a child to the current node
    public void AddChild(TreeNode<T> child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    // Removes a child from the current node
    public bool RemoveChild(TreeNode<T> child)
    {
        child.Parent = null;
        return Children.Remove(child);
    }

    // Returns true if this is a root node
    public bool IsRoot => Parent == null;

    // Returns true if this is a leaf node
    public bool IsLeaf => Children.Count == 0;

    // Returns all descendants of this node
    public IEnumerable<TreeNode<T>> GetDescendants()
    {
        foreach (var child in Children)
        {
            yield return child;

            foreach (var descendant in child.GetDescendants())
            {
                yield return descendant;
            }
        }
    }

    public override string ToString()
    {
        return Data.ToString();
    }
}