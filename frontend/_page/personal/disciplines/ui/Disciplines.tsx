"use client"

import { mockDisciplines } from "@/shared/model/mocks/mockDisciplines"
import { Sidebar } from "@/shared/ui/sidebar"
import { DisciplinesTable } from "@/widgets/disciplines-table"

const sidebarItems = [
	{
		text: "Все",
		key: "/personal/disciplines",
	},
]

const Disciplines = () => {
	return (
		<>
			<Sidebar
				items={sidebarItems}
				title="Дисциплины"
				onClose={() => {}}
				open={true}
				className="flex-1"
			/>
			<DisciplinesTable disciplines={mockDisciplines} />
		</>
	)
}

export default Disciplines
