<?php
header('Content-Type: application/json');
require_once 'db.php';

$method = $_SERVER['REQUEST_METHOD'];
$id = $_GET['id'] ?? null;

function getInputData() {
    return json_decode(file_get_contents("php://input"), true);
}

switch ($method) {
    case 'GET':
        if ($id) {
            $stmt = $pdo->prepare("SELECT * FROM children WHERE id = ?");
            $stmt->execute([$id]);
            $data = $stmt->fetch(PDO::FETCH_ASSOC);
        } else {
            $stmt = $pdo->query("SELECT * FROM children");
            $data = $stmt->fetchAll(PDO::FETCH_ASSOC);
        }
        echo json_encode($data);
        break;

    case 'POST':
        $input = getInputData();
        if ($input) {
            $columns = implode(", ", array_keys($input));
            $placeholders = implode(", ", array_fill(0, count($input), '?'));
            $stmt = $pdo->prepare("INSERT INTO children ($columns) VALUES ($placeholders)");
            if ($stmt->execute(array_values($input))) {
                echo json_encode(['success' => true, 'id' => $pdo->lastInsertId()]);
            } else {
                echo json_encode(['success' => false, 'message' => 'Failed to insert data']);
            }
        } else {
            echo json_encode(['success' => false, 'message' => 'Invalid input data']);
        }
        break;

    case 'PUT':
        if (!$id) {
            echo json_encode(['error' => 'ID required for update']);
            exit;
        }
        $input = getInputData();
        $setClause = implode(", ", array_map(fn($k) => "$k = ?", array_keys($input)));
        $stmt = $pdo->prepare("UPDATE children SET $setClause WHERE id = ?");
        $stmt->execute([...array_values($input), $id]);
        echo json_encode(['success' => true]);
        break;

    case 'DELETE':
        if (!$id) {
            echo json_encode(['error' => 'ID required for deletion']);
            exit;
        }
        $stmt = $pdo->prepare("DELETE FROM children WHERE id = ?");
        $stmt->execute([$id]);
        echo json_encode(['success' => true]);
        break;

    default:
        http_response_code(405);
        echo json_encode(['error' => 'Method not allowed']);
        break;
}
?>