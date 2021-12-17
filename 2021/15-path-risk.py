import heapq as hq

import numpy as np
from networkx import shortest_path_length, DiGraph

from aoc import *


def solve_bfs(risk):
    very_big_number = risk.sum().sum()
    total_risk = np.ones_like(risk) * very_big_number
    total_risk[0, 0] = 0
    q = [(0, 0)]
    out_q = 0
    while out_q < len(q):
        x, y = q[out_q]
        out_q += 1
        for nx, ny, nr in neighbours4(x, y, risk):
            if total_risk[nx, ny] > total_risk[x, y] + nr:
                total_risk[nx, ny] = total_risk[x, y] + nr
                q.append((nx, ny))
    return total_risk


def shift_with_padding(data, shift, axis, pad_value):
    shifted_data = np.roll(data, shift, axis=axis)
    null_slice = slice(None, None)
    if shift < 0:
        part_slice = slice(shift, None)
    else:
        part_slice = slice(None, shift)
    if axis == 1:
        full_slice = (null_slice, part_slice)
    else:
        full_slice = (part_slice, null_slice)
    shifted_data[full_slice] = pad_value
    return shifted_data


def solve_matrix(risk):
    big_number = risk.sum().sum()
    total_risk = np.ones_like(risk) * big_number
    total_risk[0, 0] = 0

    while True:
        prev = total_risk
        total_risk = np.minimum.reduce([
            total_risk,
            risk + shift_with_padding(total_risk, -1, axis=0, pad_value=big_number),
            risk + shift_with_padding(total_risk, 1, axis=0, pad_value=big_number),
            risk + shift_with_padding(total_risk, -1, axis=1, pad_value=big_number),
            risk + shift_with_padding(total_risk, 1, axis=1, pad_value=big_number)
        ])
        if np.array_equal(total_risk, prev):
            return total_risk


def solve_dijkstra(risks):
    h, w = np.shape(risks)
    q = [(0, (0, 0))]
    visited = {(0, 0)}
    while q:
        total_risk, (x, y) = hq.heappop(q)
        if (x, y) == (w - 1, h - 1):
            return total_risk
        for nx, ny, nr in neighbours4(x, y, risks):
            if (nx, ny) not in visited:
                hq.heappush(q, (total_risk + nr, (nx, ny)))
                visited.add((nx, ny))
    return risks


def extend_map_simple(risks):
    n = risks.shape[0]
    big_risks = np.zeros((n * 5, n * 5))
    for x in range(5 * n):
        for y in range(5 * n):
            d = x // n + y // n
            big_risks[x, y] = (risks[x % n, y % n] + d) % 9 + 1
    return big_risks


def extend_map_slices(risks):
    n = risks.shape[0]
    big_risks = np.zeros((n * 5, n * 5))
    for x in range(5):
        for y in range(5):
            d = x + y
            big_risks[x * n:(x + 1) * n, y * n:(y + 1) * n] = (risks + d) % 9 + 1
    return big_risks


def extend_map_concat(risks):
    risks = np.concatenate([risks + i for i in range(5)], axis=0)
    risks = np.concatenate([risks + i for i in range(5)], axis=1)
    return risks % 9 + 1


def extend_map_block(risks):
    return np.block([[(risks + y + x - 1) % 9 + 1 for x in range(5)] for y in range(5)])


def solve_networkx(risk, repeats):
    g = DiGraph()
    n = len(risk)
    for y, row in enumerate(risk):
        for x, cost in enumerate(row):
            for ix in range(repeats):
                for iy in range(repeats):
                    xx = x + ix * n
                    yy = y + iy * n
                    for nx, ny in [(xx - 1, yy), (xx + 1, yy), (xx, yy + 1), (xx, yy - 1)]:
                        if 0 <= nx < repeats*n and 0 <= ny < repeats*n:
                            real_cost = (cost + ix + iy - 1) % 9 + 1
                            g.add_edge((nx, ny), (xx, yy), weight=real_cost)
    return shortest_path_length(g, (0, 0), (n * repeats - 1, n * repeats - 1), weight="weight")


inp = np.array(read_map(), dtype=int)

print("Part One")
measure("* BFS     ", lambda: solve_bfs(inp)[-1, -1])
measure("* NetworkX", lambda: solve_networkx(inp, 1))
measure("* Dijkstra", lambda: solve_dijkstra(inp))
measure("* Matrix  ", lambda: solve_matrix(inp)[-1, -1])

print("Part Two")
measure("* BFS     ", lambda: solve_bfs(extend_map_block(inp))[-1, -1])
measure("* Matrix  ", lambda: solve_matrix(extend_map_block(inp))[-1, -1])
measure("* NetworkX", lambda: solve_networkx(inp, 5))
measure("* Dijkstra", lambda: solve_dijkstra(extend_map_block(inp)))
