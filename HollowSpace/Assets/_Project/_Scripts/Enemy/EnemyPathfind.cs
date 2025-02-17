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

    private List<Vector2> FindPath(Vector2 start, Vector2 goal) {
        start = RoundToNearestStep(start);
        goal = RoundToNearestStep(goal);

       _openSet = new List<Node> { new Node(start, null, 0, Vector2.Distance(start, goal)) };
        _closedSet = new HashSet<Vector2>();
        int iterations = 0;

        while (_openSet.Count > 0) {
            if (iterations++ > maxIterations) {
                Debug.LogWarning("Pathfinding exceeded maximum iterations.");
                break;
            }

            _openSet.Sort((a, b) => a.F.CompareTo(b.F));
            var currentNode = _openSet[0];
            _openSet.RemoveAt(0);

            if (currentNode.Position == goal) {
                var path = new List<Vector2>();
                while (currentNode != null) {
                    path.Add(currentNode.Position);
                    currentNode = currentNode.Parent;
                }

                path.Reverse();
                return path;
            }

            _closedSet.Add(currentNode.Position);

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

    private IEnumerable<Vector2> GetNeighbors(Vector2 position) {
        var directions = new Vector2[] {
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

    
    private void OnDrawGizmosSelected() {
        if (!Application.isPlaying) return;
        
        // Attacking
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _enemy.stats.attackRange);
        
        // Pathfinding
        if (_path.Count > 0) {
            Gizmos.color = Color.green;
            for (int i = 0; i < _path.Count - 1; i++) {
                Gizmos.DrawLine(_path[i], _path[i + 1]);
            }
        }
        
        // Pathfinding data
        if (_openSet != null) {
            Gizmos.color = Color.blue;
            foreach (var node in _openSet) {
                Gizmos.DrawWireSphere(node.Position, 0.1f);
            }
        }
        
        if (_closedSet != null) {
            Gizmos.color = Color.red;
            foreach (var position in _closedSet) {
                Gizmos.DrawWireSphere(position, 0.1f);
            }
        }
    }
}