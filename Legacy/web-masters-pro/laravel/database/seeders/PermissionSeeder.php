<?php

namespace Database\Seeders;

use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class PermissionSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        $groups = [
            [
                'role' => 'superuser',
                'permissions' => [
                    [
                        'model' => 'App',
                        'list' => ['pulse', 'stats']
                    ],
                    [
                        'model' => 'User',
                        'list' => ['read', 'save', 'delete', 'view-contacts'],
                    ],
                    [
                        'model' => 'Profession',
                        'list' => ['read', 'save', 'delete'],
                    ],
                    [
                        'model' => 'Order',
                        'list' => ['read', 'save', 'delete'],
                    ],
                    [
                        'model' => 'Worker',
                        'list' => ['files', 'read', 'save', 'delete'],
                    ],
                    [
                        'model' => 'Report',
                        'list' => ['accept']
                    ],
                    [
                        'model' => 'Ticket',
                        'list' => ['create', 'read', 'send', 'handle']
                    ],
                    [
                        'model' => 'Customer',
                        'list' => ['read', 'save', 'delete']
                    ],
                    [
                        'model' => 'File',
                        'list' => ['read']
                    ],
                    [
                        'model' => 'PaymentRequest',
                        'list' => ['read']
                    ],
                    [
                        'model' => 'PlatformSetting',
                        'list' => ['read', 'save']
                    ]
                ]
            ],
            [
                'role' => 'admin',
                'permissions' => [
                    [
                        'model' => 'App',
                        'list' => ['stats']
                    ],
                    [
                        'model' => 'User',
                        'list' => ['read', 'save', 'delete', 'view-contacts'],
                    ],
                    [
                        'model' => 'Profession',
                        'list' => ['read', 'save', 'delete'],
                    ],
                    [
                        'model' => 'Order',
                        'list' => ['read', 'save', 'delete'],
                    ],
                    [
                        'model' => 'Worker',
                        'list' => ['files', 'read', 'save', 'delete'],
                    ],
                    [
                        'model' => 'Report',
                        'list' => ['accept']
                    ],
                    [
                        'model' => 'Ticket',
                        'list' => ['read', 'send', 'handle']
                    ],
                    [
                        'model' => 'Customer',
                        'list' => ['read', 'save', 'delete']
                    ],
                    [
                        'model' => 'File',
                        'list' => ['read']
                    ],
                    [
                        'model' => 'PaymentRequest',
                        'list' => ['read']
                    ],
                    [
                        'model' => 'PlatformSetting',
                        'list' => ['read', 'save']
                    ]
                ]
            ],
            [
                'role' => 'manager',
                'permissions' => [
                    [
                        'model' => 'User',
                        'list' => ['view-contacts'],
                    ],
                    [
                        'model' => 'Ticket',
                        'list' => ['create', 'read', 'send', 'handle']
                    ],
                    [
                        'model' => 'File',
                        'list' => ['read']
                    ],
                    [
                        'model' => 'Worker',
                        'list' => ['files', 'read', 'save'],
                    ],
                    [
                        'model' => 'Order',
                        'list' => ['read'],
                    ],
                    [
                        'model' => 'PaymentRequest',
                        'list' => ['read']
                    ]
                ]
            ],
            [
                'role' => 'customer',
                'permissions' => [
                    [
                        'model' => 'Order',
                        'list' => ['place']
                    ],
                    [
                        'model' => 'Report',
                        'list' => ['accept']
                    ],
                    [
                        'model' => 'Ticket',
                        'list' => ['create', 'read', 'send']
                    ]
                ]
            ],
            [
                'role' => 'worker',
                'permissions' => [
                    [
                        'model' => 'Work',
                        'list' => ['accept']
                    ],
                    [
                        'model' => 'Ticket',
                        'list' => ['create', 'read', 'send']
                    ]
                ]
            ],
        ];

        DB::table('permissions')->truncate();

        foreach ($groups as $group)
        {
            foreach ($group['permissions'] as $permissions)
            {
                foreach ($permissions['list'] as $action)
                {
                    DB::table('permissions')->insert([
                        'role' => $group['role'],
                        'model' => $permissions['model'],
                        'action' => $action
                    ]);
                }
            }
        }
    }
}
