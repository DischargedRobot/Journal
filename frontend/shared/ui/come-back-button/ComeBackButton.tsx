import ArrowBack from "@mui/icons-material/ArrowBack"
import IconButton, { type IconButtonProps } from "@mui/material/IconButton"
import { memo } from "react"


export type ComeBackButtonProps = {
	label?: string
} & IconButtonProps

const ComeBackButton = ({
	label = "Назад",
	...props
}: ComeBackButtonProps) => {
	return (
		<IconButton aria-label={label} {...props}>
			<ArrowBack />
		</IconButton>
	)
}

export default memo(ComeBackButton)
