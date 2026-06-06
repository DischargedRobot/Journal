import AddIcon from "@mui/icons-material/Add"
import Chip, { ChipProps } from "@mui/material/Chip"

const AddChip = ({
	label = "Добавить",
	className = "border-2",
	sx,
	icon = <AddIcon />,
	clickable = true,
	...rest
}: ChipProps) => {
	return (
		<Chip
			clickable={clickable}
			icon={icon}
			label={label}
			className={className}
			sx={[
				{
					borderColor: "secondary.dark",
					backgroundColor: "secondary.light",
					"&:hover": {
						backgroundColor: "primary.main",
						color: "primary.contrastText",
						"& .MuiSvgIcon-root": {
							color: "primary.contrastText",
						},
					},
				},
				...(Array.isArray(sx) ? sx : sx ? [sx] : []),
			]}
			{...rest}
		/>
	)
}

export default AddChip
