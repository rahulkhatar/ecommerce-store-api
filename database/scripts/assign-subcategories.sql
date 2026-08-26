SET QUOTED_IDENTIFIER ON;
GO

-- Sports & Outdoors -> type subcategories
UPDATE Products SET CategoryId = '2F7B1012-0A99-46BF-8B54-14FAC01E4229' WHERE Id = '5138406C-EBFB-47B4-B7B1-74EBE71262DC'; -- Adjustable Sports Cap -> Caps
UPDATE Products SET CategoryId = '151CBB55-F747-47DE-9A93-6DDD3EC9E6C1' WHERE Id = 'ED47DF4F-0A68-4585-B06A-D53C6EC1E35C'; -- Inline Roller Skates -> Skates
UPDATE Products SET CategoryId = 'B99F3475-1AD2-4658-9BE0-C92BB93D801B' WHERE Id = '132282B0-F3BD-4F3E-B565-A6F1C4D9243B'; -- Aluminum Baseball Bat -> Baseball
UPDATE Products SET CategoryId = '5E073CA0-ACD1-4CB0-AAF1-8119ECDE01B8' WHERE Id = '388A0DBA-265E-494F-A7CC-0B9FB3E5B168'; -- Sports Duffel Kit Bag -> Kit Bags
UPDATE Products SET CategoryId = '8CECAF2F-2483-4826-871C-6EE14A50BF38' WHERE Id = '6CAE5DFE-E0E5-45CC-A474-51F2C202B846'; -- Professional Volleyball -> Volleyball
UPDATE Products SET CategoryId = 'DDACA870-7763-4BEA-B3BC-91134B7C38DD' WHERE Id = '80DB7365-8191-4983-B86B-783F982C5403'; -- Official Size 5 Football -> Football

-- Electronics -> type subcategories
UPDATE Products SET CategoryId = '8776E838-B849-460A-9E03-629CC573D494' WHERE Id = 'F3ED01A3-498D-42C3-B40E-B8D1A89218ED'; -- Wireless Bluetooth Headphones -> Audio
UPDATE Products SET CategoryId = 'E34C5D12-2181-40D4-A11E-77B3916A8697' WHERE Id = 'E5D942C5-8281-4EF7-ABB6-C99AF595FC3C'; -- 4K Webcam -> Webcams
UPDATE Products SET CategoryId = '2FC1F696-6705-481A-9152-9B7621F9B88E' WHERE Id = '880880C6-57A1-47AA-A829-8E4ACA764BE5'; -- Portable Power Bank -> Power & Charging
UPDATE Products SET CategoryId = '2FC1F696-6705-481A-9152-9B7621F9B88E' WHERE Id = '2E292F45-2B44-4B18-8817-64059D1AF63A'; -- USB-C Fast Charging Cable -> Power & Charging
UPDATE Products SET CategoryId = '9775E938-7DA0-40D4-8D3D-9AA828591F5D' WHERE Id = '7D909E7D-FB0E-4622-BF58-5705B823D54E'; -- Wireless Mouse -> Computer Accessories

-- Clothing -> type subcategories
UPDATE Products SET CategoryId = '0CFEC799-4DB1-405D-AF63-B8A687504CB9' WHERE Id = '281C98AB-444A-4865-BDD3-D6682656F4F6'; -- Winter Wool Sweater -> Sweaters
UPDATE Products SET CategoryId = '8AB862EC-738A-4887-8A99-1A461CBB0EE4' WHERE Id = '3DC40ED4-836F-4DA0-87AB-81E194D23C00'; -- Premium Cotton T-Shirt -> T-Shirts
UPDATE Products SET CategoryId = '30EEABAF-EF84-4671-8EDA-D2298E3C63A2' WHERE Id = 'BACD3CD6-248D-4004-BC41-6C485D117E7B'; -- Casual Denim Jeans -> Jeans

-- Home & Garden: re-home the misfiled desk lamp out of Electronics
UPDATE Products SET CategoryId = '1B5C0F51-81CD-45CD-A621-0F88A7F3C3DC' WHERE Id = '93A5AFDC-CEF8-426E-B85A-B6B33767C67F'; -- Crystal Glass Desk Lamp -> Lighting & Decor

-- Dev/test artifacts that shouldn't be visible in production - soft-hide, don't delete.
UPDATE Products SET IsActive = 0 WHERE Id IN ('CA97E841-6446-4E50-B466-E8E4EC40F0C4', '25D718F5-111D-4711-B316-B7799ADB44B8', 'D7002934-DD73-44A5-8562-125EE91575C2');
GO

SELECT p.Name, c.Name AS Category, p.Vendor, p.IsActive FROM Products p JOIN Categories c ON c.Id = p.CategoryId ORDER BY c.Name, p.Name;
GO
