"use client"

import { TDiscipline } from "@/shared/model/discipline"
import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import Accordion from "@mui/material/Accordion"
import AccordionSummary from "@mui/material/AccordionSummary"
import Typography from "@mui/material/Typography"
import ExpandMoreIcon from "@mui/icons-material/ExpandMore"
import AccordionDetails from "@mui/material/AccordionDetails"
import { useMemo } from "react"

interface Props {
	disciplines: TDiscipline[]
}

const DisciplinesTable = ({ disciplines }: Props) => {
	const groupedDisciplines = useMemo(
		() =>
			disciplines.reduce<Record<string, TDiscipline[]>>(
				(acc, discipline) => {
					if (!acc[discipline.name]) {
						acc[discipline.name] = []
					}
					acc[discipline.name].push(discipline)
					return acc
				},
				{},
			),
		[disciplines],
	)

	return (
		<List className="w-full overflow-y-auto scrollbar-gutter-stable">
			{Object.entries(groupedDisciplines).map(([name, disciplines]) => (
				<ListItem key={name} className="w-full">
					<Accordion className="w-full">
						<AccordionSummary expandIcon={<ExpandMoreIcon />}>
							<Typography>{name}</Typography>
						</AccordionSummary>
						<AccordionDetails>
							<ul className="flex flex-col gap-2 w-full">
								{disciplines.map((discipline) => (
									<Accordion
										key={discipline.uuid}
										className="w-full"
										component="li"
										variant="outlined"
									>
										<AccordionSummary
											expandIcon={<ExpandMoreIcon />}
										>
											<Typography>
												{discipline.type}
											</Typography>
										</AccordionSummary>
										<AccordionDetails>
											{discipline.groups.map((group) => (
												<Typography key={group.uuid}>
													{group.name}
												</Typography>
											))}
										</AccordionDetails>
									</Accordion>
								))}
							</ul>
						</AccordionDetails>
					</Accordion>
				</ListItem>
			))}
		</List>
	)
}

export default DisciplinesTable
