import { CheckboxRect } from "@/shared/ui/checkbox"
import Box from "@mui/material/Box"
import Popper from "@mui/material/Popper"
import Typography from "@mui/material/Typography"
import { TPresencesStatus } from "@/shared/model/presences-status"

interface Props {
	isOpen: boolean
	anchorEl: HTMLElement | null
	absenceStatusDenominator: number
	selectedStatus?: TPresencesStatus
	onChange: (event: React.ChangeEvent<HTMLInputElement>, status: TPresencesStatus) => void
}

const parseAbsenceStatus = (status: TPresencesStatus | undefined, absenceStatusDenominator: number) => {
	if (!status) {
		return null
	}
	const match = /^(\d+)\/(\d+)$/.exec(status)
	if (!match || Number(match[2]) !== absenceStatusDenominator) {
		return null
	}

	return Number(match[1])
}

const SelectAbsenceStatus = (props: Props) => {
	const {
		isOpen,
		anchorEl,
		absenceStatusDenominator,
		selectedStatus,
		onChange,
	} = props

	const selectedAbsenceNumerator = parseAbsenceStatus(selectedStatus, absenceStatusDenominator)
	const absenceNumerators = Array.from(
		{ length: absenceStatusDenominator - 1 },
		(_, index) => index + 1,
	)

	return (
		<Popper
			open={isOpen}
			anchorEl={anchorEl}
			placement="right-start"
			className="z-100"
			modifiers={[
				{
					name: "offset",
					options: { offset: [0, 8] },
				},
				{
					// чтобы не вылезало за границы экрана - разрешение на попытку встать в другое положение
					name: "preventOverflow",
					options: {
						boundary: "viewport", // границы экрана
						padding: 8,
					},
				},
				{
					// если не влезает, то переворачивается в другое положение в таком порядке
					name: "flip",
					options: {
						fallbackPlacements: ["left-start", "right-end", "left-end"],
					},
				},
			]}
		>
			<Box
				data-absence-status-popper
				className="relative grid grid-cols-[1fr_auto] gap-2 p-3 rounded-lg"
				sx={{
					backgroundColor: "secondary.light",
					boxShadow: "var(--shadow)",
					"&::after": {
						content: '""',
						position: "absolute",
						left: -6,
						top: 21,
						width: 0,
						height: 0,
						borderTop: "6px solid transparent",
						borderBottom: "6px solid transparent",
						borderRight: "6px solid",
						borderRightColor: "secondary.light",
					},
				}}
			>
				<Box />
				<Typography className="leading-none justify-self-center px-0.5">
					{absenceStatusDenominator}
				</Typography>
				{absenceNumerators.map((absenceNumerator) => (
					<Box key={absenceNumerator} className="contents">
						<Typography className="leading-none">
							{absenceNumerator} /
						</Typography>
						<CheckboxRect
							checked={selectedAbsenceNumerator === absenceNumerator}
							onChange={(event) => onChange(event, `${absenceNumerator}/${absenceStatusDenominator}`)}
						/>
					</Box>
				))}
			</Box>
		</Popper>
	)
}

export default SelectAbsenceStatus
