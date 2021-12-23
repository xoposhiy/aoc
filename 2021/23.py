import heapq as hq
import time

price = [1, 10, 100, 1000]


def get_cost(x0, x1, xt):
    return max(0, x1 - max(x0, xt), min(x0, xt) - x1)

class State:
    def __init__(self, stacks, corridor, spent, full_cost, prev_state, stack_size):
        self.stacks = stacks
        self.corridor = corridor
        self.spent = spent
        self.full_cost = full_cost
        self.prev_state = prev_state
        self.stack_size = stack_size

    def __eq__(self, other):
        return self.stacks == other.stacks and self.corridor == other.corridor

    def __hash__(self):
        return hash(tuple(self.corridor))

    def __lt__(self, other):
        return self.spent - other.spent < 0

    def __str__(self):
        res = f'{self.spent, self.full_cost}\n'
        for v in self.corridor:
            res += str(v) if v is not None else '.'
        res += '\n'
        for y in range(self.stack_size):
            res += '  '
            for stack in self.stacks:
                if len(stack) < self.stack_size - y:
                    res += '. '
                else:
                    res += str(stack[self.stack_size - 1 - y]) + ' '
            res += '\n'
        return res

    def moves(self):
        yield from self.in_moves()
        yield from self.out_moves()

    def out_moves(self):
        for x, stack in enumerate(self.stacks):
            if all(c == x for c in stack):
                continue
            cx = 2 + x * 2
            tx = cx - 1
            while tx >= 0 and self.corridor[tx] is None:
                if tx not in [2, 4, 6, 8]:
                    yield 'out', x, tx
                tx -= 1

            tx = cx + 1
            while tx < len(self.corridor) and self.corridor[tx] is None:
                if tx not in [2, 4, 6, 8]:
                    yield 'out', x, tx
                tx += 1

    def in_moves(self):
        for corridor_pos, v in enumerate(self.corridor):
            if v is not None and self.can_move_in_room(corridor_pos, v):
                yield 'in', corridor_pos, v

    def can_move_in_room(self, corridor_x, stack_index):
        stack_is_ready = all(v == stack_index for v in self.stacks[stack_index])
        if not stack_is_ready:
            return False
        tx = stack_index * 2 + 2
        way_is_free = all(pos == corridor_x or self.corridor[pos] is None for pos in
                          range(min(tx, corridor_x), max(tx, corridor_x) + 1))
        return way_is_free

    def apply_move(self, move):
        corridor = self.corridor.copy()
        match move:
            case 'out', stack_index, pos:
                stacks = [stack.copy() if i == stack_index else stack for i, stack in enumerate(self.stacks)]
                stack = stacks[stack_index]
                v = stack.pop()
                corridor[pos] = v
                cost = get_cost(stack_index * 2 + 2, pos, v * 2 + 2) * price[v]
                full_cost = (self.stack_size - len(stack) + abs(stack_index * 2 + 2 - pos)) * price[v]
                return State(stacks, corridor, self.spent + cost, self.full_cost + full_cost, self, self.stack_size)
            case 'in', pos, stack_index:
                stacks = [stack.copy() if i == stack_index else stack for i, stack in enumerate(self.stacks)]
                stack = stacks[stack_index]
                v = corridor[pos]
                corridor[pos] = None
                stack.append(v)
                full_cost = (self.stack_size - len(self.stacks[stack_index]) + abs(stack_index * 2 + 2 - pos)) * price[v]
                return State(stacks, corridor, self.spent, self.full_cost + full_cost, self, self.stack_size)

    def done(self):
        for stack_index, stack in enumerate(self.stacks):
            if stack != [stack_index] * self.stack_size:
                return False
        return True


def solve_dijkstra(state):
    q = [state]
    visited = {state}
    while q:
        state = hq.heappop(q)
        if state.done():
            return state, visited
        for move in state.moves():
            next_state = state.apply_move(move)
            if next_state not in visited:
                hq.heappush(q, next_state)
                visited.add(next_state)
    return state, visited


def print_history(state):
    while state is not None:
        print(state)
        state = state.prev_state


s = State([[1, 3], [0, 0], [3, 1], [2, 2]], [None] * 11, 0, 0, None, 2)
print(s)
s, vs = solve_dijkstra(s)
print("Part one", s, len(vs))

start = time.time()
s = State([[1, 3, 3, 3], [0, 1, 2, 0], [3, 0, 1, 1], [2, 2, 0, 2]], [None] * 11, 0, 0, None, 4)
# s = State([[3, 3, 3, 3], [2, 1, 2, 0], [1, 0, 1, 2], [1, 2, 0, 0]], [None] * 11, 0, 0, None, 4)
print(s)
s, vs = solve_dijkstra(s)
print("Part two", s, len(vs))
print(time.time() - start)
# print_history(s)
