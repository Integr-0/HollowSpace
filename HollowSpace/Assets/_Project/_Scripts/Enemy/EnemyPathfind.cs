using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Enemy))]
public class EnemyPathfind : MonoBehaviour {
    private class Node {
        public Vector2 Position;
        public Node Parent;
        public float G; // Cost from start to current node
        public float H; // Heuristic cost from current node to goal
        public float F; // Total cost (G + H)

        public Node(Vector2 position, Node parent, float g, float h) {
            Position = position;
            Parent = parent;
            G = g;
            H = h;
            F = g + h;
        }
    }
    
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float stepSize = 0.5f;
    [SerializeField] private int maxIterations = 1000;
    
    [Header("Gizmos")]
    [SerializeField] private bool drawOpenNodes;
    [SerializeField] private bool drawClosedNodes;
    [SerializeField] private bool drawPath;
    [SerializeField] private bool drawAttackRange;
    [SerializeField] private bool drawTree;
    
    private Enemy _enemy;
    private float _cooldown;
    private Health _targetHealth;
    private List<Vector2> _path;
    private int _currentPathIndex;
    private Vector2 _lastTargetPosition;
    
    // Pathfinding data for Gizmos
    private List<Node> _openSet;
    private HashSet<Vector2> _closedSet;

    private void Start() {
        _enemy = GetComponent<Enemy>();
        _cooldown = _enemy.stats.attackCooldown;
        _targetHealth = target?.GetComponent<Health>();
        _path = new List<Vector2>();
        _lastTargetPosition = target.position;
        
        // Initialize path
        _path = FindPath(transform.position, target.position);
    }

    private void Update() {
        if (!target) return;

        if (IsInAttackRange()) Attack();
        else Pathfind();

        // Cooldown
        _cooldown -= Time.deltaTime;
    }

    private void Pathfind() {
        // Only move in darkness
        if (LightChecker.IsIlluminatedCachedNoCast(transform.position)) return;

        // Check if the target has moved significantly
        if (Vector2.Distance(_lastTargetPosition, target.position) > stepSize) {
            _path = FindPath(transform.position, target.position);
            _currentPathIndex = 0;
            _lastTargetPosition = target.position;
        }

        if (_path.Count > 0 && _currentPathIndex < _path.Count) {
            Vector2 nextPosition = _path[_currentPathIndex];
            if (Vector2.Distance(transform.position, nextPosition) < 0.1f) {
                _currentPathIndex++;
            } else {
                var direction = (nextPosition - (Vector2)transform.position).normalized;
                transform.position += _enemy.stats.speed * Time.deltaTime * (Vector3)direction;
            }
        }
    }

    private void Attack() {
        if (_cooldown > 0) return;

        _cooldown = _enemy.stats.attackCooldown;

        // Deal damage to the target
        if (!_targetHealth) {
            Debug.LogWarning($"Tried to attack {target.name}, but it has no Health component.");
            return;
        }

        _targetHealth.TakeDamage(_enemy.stats.damage);
    }

    private List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        if (!IsAccessible(goal)) return new List<Vector2>();
        if (CanJump(start, goal)) return new List<Vector2> { start, goal };
        
        start = RoundToNearestStep(start);
        goal = RoundToNearestStep(goal);

        _openSet = new List<Node> { new(start, null, 0, Vector2.Distance(start, goal)) };
        _closedSet = new HashSet<Vector2>();
        var iterations = 0;

        while (_openSet.Count > 0) {
            if (iterations++ > maxIterations) {
                Debug.LogWarning("Pathfinding exceeded maximum iterations.");
                break;
            }

            var bestNodeIndex = -1;
            var bestF = float.MaxValue;
            var bestH = float.MaxValue;
            
            for (var i = 0; i < _openSet.Count; i++)
            {
                if (_closedSet.Contains(_openSet[i].Position)) continue;
                var node = _openSet[i];
                if (node.F < bestF) {
                    bestF = node.F;
                    bestH = node.G;
                    bestNodeIndex = i;
                } else if (Mathf.Approximately(node.F, bestF) && node.H < bestH) {
                    bestH = node.G;
                    bestF = node.F;
                    bestNodeIndex = i;
                }
            }
            
            var currentNode = _openSet[bestNodeIndex];
            _openSet.RemoveAt(bestNodeIndex);
            _closedSet.Add(currentNode.Position);

            if (currentNode.Position == goal) {
                var path = new List<Vector2>();
                while (currentNode != null) {
                    path.Add(currentNode.Position);
                    currentNode = currentNode.Parent;
                }

                path.Reverse();
                
                var currentPosition = 0;
                while (path[currentPosition] != goal)
                {
                    var jumpEnd = -1;
                    for (var i = currentPosition + 1; i < path.Count; i++)
                    {
                        if (CanJump(path[currentPosition], path[i]))
                        {
                            jumpEnd = i;
                        }
                    }

                    if (jumpEnd != -1)
                    {
                        path.RemoveRange(currentPosition + 1, jumpEnd - currentPosition - 1);
                        currentPosition++;
                    }
                    else
                    {
                        currentPosition++;
                    }
                }

                return path;
            }


            foreach (var neighbor in GetNeighbors(currentNode.Position)) {
                if (_closedSet.Contains(neighbor) || IsIlluminated(neighbor)) continue;

                var tentativeG = currentNode.G + Vector2.Distance(currentNode.Position, neighbor);
                var existingNode = _openSet.Find(n => n.Position == neighbor);

                if (existingNode == null) {
                    _openSet.Add(new Node(neighbor, currentNode, tentativeG, Vector2.Distance(neighbor, goal)));
                }
                else if (tentativeG < existingNode.G) {
                    existingNode.Parent = currentNode;
                    existingNode.G = tentativeG;
                    existingNode.F = tentativeG + existingNode.H;
                }
            }
        }

        Debug.LogWarning("Pathfinding failed to find a path.");
        return new List<Vector2>();
    }
    
    private bool CanJump(Vector2 a, Vector2 b)
    {
        // check if there is a way from a to b that doesnt go trough obstacles and light
        var direction = (b - a).normalized;
        var distance = Vector2.Distance(a, b);
        var step = stepSize;
        var currentPosition = a;
        while (Vector2.Distance(currentPosition, b) > step)
        {
            currentPosition += direction * step;
            if (Physics2D.OverlapCircle(currentPosition, 0.1f, obstacleLayer) || IsIlluminated(currentPosition))
            {
                return false;
            }
        }
        
        return true;
    }
    
    private bool IsAccessible(Vector2 position) {
        return !Physics2D.OverlapCircle(position, 0.1f, obstacleLayer) && !IsIlluminated(position);
    }

    private IEnumerable<Vector2> GetNeighbors(Vector2 position) {
        var directions = new[] {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) // Diagonal directions
        };

        foreach (var direction in directions) {
            var neighborPos = position + direction * stepSize;
            if (!Physics2D.OverlapCircle(neighborPos, 0.1f, obstacleLayer) && !IsIlluminated(neighborPos)) {
                yield return neighborPos;
            }
        }
    }

    private bool IsIlluminated(Vector2 p) => LightChecker.IsIlluminatedPathfindPoint(p);
    private Vector2 RoundToNearestStep(Vector2 position) {
        return new Vector2(
            Mathf.Round(position.x / stepSize) * stepSize,
            Mathf.Round(position.y / stepSize) * stepSize
        );
    }
    private bool IsInAttackRange() {
        return (transform.position - target.position).sqrMagnitude <=
               _enemy.stats.attackRange * _enemy.stats.attackRange;
    }

    
    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        
        // Attacking
        if (drawAttackRange)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _enemy.stats.attackRange);
        }
        
        // Pathfinding data
        if (_openSet != null && drawOpenNodes) {
            Gizmos.color = Color.blue;
            foreach (var node in _openSet) {
                Gizmos.DrawWireSphere(node.Position, 0.1f);
                if (node.Parent == null || !drawTree) continue;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(node.Position, node.Parent.Position);
            }
        }
        
        if (_closedSet != null && drawClosedNodes) {
            Gizmos.color = Color.red;
            foreach (var position in _closedSet) {
                Gizmos.DrawWireSphere(position, 0.1f);
            }
        }
        
        // Pathfinding
        if (_path.Count > 0 && drawPath) {
            Gizmos.color = Color.green;
            for (int i = 0; i < _path.Count - 1; i++) {
                Gizmos.DrawLine(_path[i], _path[i + 1]);
            }
        }
    }
}