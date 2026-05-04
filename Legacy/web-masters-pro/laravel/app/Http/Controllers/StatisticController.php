<?php

namespace App\Http\Controllers;

use App\Models\Customer;
use App\Models\Order;
use App\Models\Worker;
use Illuminate\Http\Request;

class StatisticController extends Controller
{
    public function getStatistics(Request $request) {
        // Get orders count per month for the current year
        $ordersByMonth = Order::selectRaw('MONTH(created_at) as month, COUNT(*) as count')
            ->whereYear('created_at', now()->year)
            ->groupBy('month')
            ->orderBy('month')
            ->pluck('count', 'month')
            ->toArray();

        // Fill missing months with 0
        $ordersPerMonth = [];
        for ($m = 1; $m <= 12; $m++) {
            $ordersPerMonth[] = $ordersByMonth[$m] ?? 0;
        }

        return [
            'orders' => Order::count(),
            'completed' => Order::where('status', 'completed')->count(),
            'hold_balance' => Customer::sum('hold_balance'),
            'paid' => Order::where('status', 'completed')->sum('total_price'),
            'workers' => Worker::count(),
            'customers' => Customer::count(),
            'orders_per_month' => $ordersPerMonth
        ];
    }
}
